# Thiết Kế Dữ Liệu — Hệ Thống Đặt Sân

## Mục lục

1. [Tài khoản và người dùng](#1-tài-khoản-và-người-dùng)
2. [Quản lý sân](#2-quản-lý-sân)
3. [Đặt sân](#3-đặt-sân)
4. [Hệ thống](#4-hệ-thống)

---

## 1. Tài khoản và người dùng

### Business Rules

- **BR-01**: Một `UserAccount` chỉ có thể liên kết với một `Employee` hoặc một `Customer`, không được phép liên kết cả
  hai.
- **BR-02**: Tài khoản Employee chỉ được tạo bởi Admin. Tài khoản Customer được tạo tự động khi đăng ký qua App/Web,
  hoặc được nhân viên tạo thủ công khi khách đến lần đầu.
- **BR-03**: Khi tạo một `Booking`, trường `customerId` là bắt buộc và phải tham chiếu đến một bản ghi `Customer` hợp
  lệ. Nếu khách chưa tồn tại trong hệ thống, phải tạo mới `Customer` với `accountId = NULL` và các thông tin từ form đặt
  sân.

### Entities

**UserAccount** (Tài khoản người dùng)

| Field        | Type                | Ghi chú                                          |
|--------------|---------------------|--------------------------------------------------|
| accountId    | uuid                | PK                                               |
| loginEmail   | string              | Tối ưu query đăng nhập                           |
| passwordHash | string              |                                                  |
| accountType  | Enum[AccountType]   | 1 tài khoản chỉ có thể là Employee hoặc Customer |
| status       | Enum[AccountStatus] |                                                  |
| createdAt    | DateTime            |                                                  |
| updatedAt    | DateTime            |                                                  |

**Employee** (Thông tin nhân viên)

| Field      | Type         | Ghi chú                            |
|------------|--------------|------------------------------------|
| employeeId | uuid         | PK                                 |
| accountId  | uuid, NULL   | FK → UserAccount.accountId, UNIQUE |
| fullName   | string       |                                    |
| phone      | string       |                                    |
| email      | string, NULL | UNIQUE khi khác NULL               |
| isAdmin    | bool         |                                    |
| createdAt  | DateTime     |                                    |
| updatedAt  | DateTime     |                                    |

**Customer** (Thông tin khách hàng)

| Field      | Type       | Ghi chú                            |
|------------|------------|------------------------------------|
| customerId | uuid       | PK                                 |
| accountId  | uuid, NULL | FK → UserAccount.accountId, UNIQUE |
| fullName   | string     |                                    |
| phone      | string     |                                    |
| email      | string     |                                    |
| createdAt  | DateTime   |                                    |
| updatedAt  | DateTime   |                                    |

**VerificationCode** (Mã xác minh email hoặc đặt lại mật khẩu)

| Field     | Type           | Ghi chú                         |
|-----------|----------------|---------------------------------|
| email     | string         | PK thành phần                   |
| type      | Enum[CodeType] | PK thành phần                   |
| codeHash  | string         | Chỉ lưu mã đã băm               |
| attempt   | int            | Số lần nhập thử                 |
| isUsed    | bool           | Đánh dấu mã đã sử dụng          |
| expiresAt | DateTime       | Thời điểm hết hạn, lưu theo UTC |

### Enums

| Enum          | Values                           |
|---------------|----------------------------------|
| AccountStatus | `ACTIVE`, `DISABLED`             |
| AccountType   | `CUSTOMER`, `EMPLOYEE`           |
| CodeType      | `VERIFY_EMAIL`, `RESET_PASSWORD` |

> Các enum nghiệp vụ được khai báo bằng PascalCase trong C#. `AccountStatus` và `AccountType` được EF Core lưu dưới dạng chuỗi; `CodeType` hiện được lưu dưới dạng số nguyên.

### Entity Constraints

**UserAccount**

```sql
UNIQUE (loginEmail)
```

**Employee**

```sql
UNIQUE (accountId)
UNIQUE (email) WHERE email IS NOT NULL
```

**Customer**

```sql
UNIQUE (accountId)
UNIQUE (email)
```

**VerificationCode**

```sql
PRIMARY KEY (email, type)
```

---

## 2. Quản lý sân

### Business Rules

- **BR-04**: Khi tạo/sửa một `Booking`, thời gian đặt (`date`, `startTime`, `endTime`) bắt buộc phải nằm gọn trong một
  hoặc nhiều khung `CourtSchedule` tương ứng với `courtId` và `dayOfWeek` đó, với điều kiện `isAvailable = true`.
- **BR-05**: Không được phép xóa hoặc sửa `CourtSchedule` nếu đang có `Booking` ở trạng thái `PENDING` hoặc `CONFIRMED`
  nằm trong khoảng thời gian bị ảnh hưởng.
- **BR-06**: Dữ liệu trong `PricingRule` không được chồng chéo. Toàn bộ các khung giờ trong `CourtSchedule` phải được
  bao phủ bởi ít nhất 1 `PricingRule` để tránh lỗi tính tiền khi đặt sân.
- **BR-07**: `pricePerHour` được tính theo số phút thực tế nằm trong khung giờ quy định.
  > Ví dụ: Đặt 16:30–18:00, giá 06:00–17:00 là 60k/h và 17:00–22:00 là 100k/h.
  > Tổng tiền = (30 phút × 60k/60) + (60 phút × 100k/60) = 30k + 100k = **130k**
- **BR-08**: `dayOfWeek` dùng ISO-8601: 1=Monday...7=Sunday

### Entities

**BadmintonCourt** (Danh sách sân)

| Field       | Type              | Ghi chú |
|-------------|-------------------|---------|
| courtId     | int               | PK      |
| name        | string            | UNIQUE  |
| description | string            |         |
| status      | Enum[CourtStatus] |         |
| createdAt   | DateTime          |         |
| updatedAt   | DateTime          |         |

**CourtSchedule** (Lịch mở/đóng sân theo từng ngày trong tuần; tự động sinh từ thứ 2 đến chủ nhật)

| Field       | Type     | Ghi chú                     |
|-------------|----------|-----------------------------|
| scheduleId  | int      | PK                          |
| courtId     | int      | FK → BadmintonCourt.courtId |
| dayOfWeek   | int      | ISO-8601, từ 1 đến 7       |
| openTime    | TimeOnly |                             |
| closeTime   | TimeOnly |                             |
| isAvailable | bool     |                             |
| createdAt   | DateTime |                             |
| updatedAt   | DateTime |                             |

**PricingRule** (Giá bắt đầu áp dụng từ một mốc giờ)

| Field         | Type          | Ghi chú                           |
|---------------|---------------|-----------------------------------|
| pricingRuleId | int           | PK                                |
| courtId       | int           | FK → BadmintonCourt.courtId       |
| startTime     | TimeOnly      | Có hiệu lực đến mốc kế tiếp hoặc giờ đóng sân |
| pricePerHour  | Decimal(10,2) |                                   |
| dayOfWeek     | int           | ISO-8601, từ 1 đến 7              |
| createdAt     | DateTime      |                                   |
| updatedAt     | DateTime      |                                   |

### Entity Constraints

**CourtSchedule**

```sql
UNIQUE (courtId, dayOfWeek, openTime)
CHECK (openTime < closeTime)
EXCLUDE USING gist (
  courtId WITH =,
  dayOfWeek WITH =,
  tsrange(openTime, closeTime) WITH &&
)
```

**BadmintonCourt**

```sql
UNIQUE (name)
```

**PricingRule**

```sql
UNIQUE (courtId, dayOfWeek, startTime)
```

`PricingRule` không lưu `endTime`. Cuối mỗi khung giá được suy ra từ `startTime` của khung kế tiếp; khung cuối kết thúc tại `CourtSchedule.closeTime`. Application yêu cầu mốc đầu tiên bằng `openTime`, mọi mốc nằm trong lịch mở sân, không trùng nhau và giá lớn hơn 0.

### Enums

| Enum        | Values                            |
|-------------|-----------------------------------|
| CourtStatus | `ACTIVE`, `MAINTENANCE`, `CLOSED` |

---

## 3. Đặt sân

### Business Rules

- **BR-09**: Một sân (`courtId`) tại cùng một thời điểm (`date`, `startTime`–`endTime`) chỉ được phép có duy nhất 1
  `Booking` không ở trạng thái `CANCELLED`. Hệ thống phải kiểm tra điều này trước khi tạo hoặc xác nhận đặt sân.
- **BR-10**: Mỗi lần thay đổi trạng thái của `Booking`, bắt buộc phải ghi log vào bảng `BookingStatusHistory` với
  `changedByEmployee` là `EmployeeId` (nếu nhân viên thao tác) hoặc `NULL` (nếu hệ thống tự động), kèm theo lý do (`reason`).
- **BR-11**: `totalCost` trong bảng `Booking` được tính ngay tại thời điểm tạo (hoặc xác nhận) dựa trên `PricingRule`
  hiện hành và được lưu cứng (snapshot) vào bảng `Booking` để giá tiền không bị thay đổi khi sửa bảng giá sau này.
- **BR-12**: `Invoice` được hệ thống **tự động tạo** ngay khi `Booking` chuyển sang `CONFIRMED` hoặc `COMPLETED` (nếu
  `Booking` **chưa từng có Invoice hợp lệ**)
    - Chuyển sang **`CONFIRMED`**: dùng khi khách đã thanh toán trước khi sử dụng sân — chuyển khoản (khi có tính năng),
      hoặc tiền mặt tại quầy trước ít nhất 1 ngày so với ngày dùng sân.
    - Chuyển sang **`COMPLETED`**: dùng khi khách thanh toán vào ngày dùng sân, hoặc khi việc đặt/sử dụng/thanh toán
      được nhân viên ghi nhận cùng lúc sau khi đã diễn ra.
    - Luồng trạng thái không nhất thiết tuyến tính: `PENDING → CONFIRMED → COMPLETED` (đặt trước, trả trước) và
      `PENDING → COMPLETED` (bỏ qua CONFIRMED, ghi nhận sau khi đã sử dụng/thanh toán) đều hợp lệ.
- **BR-13**: `invoiceCode` được sinh tự động theo định dạng `HD-{yyyyMMdd}-{STT}` (VD: `HD-20260512-0001`). Mỗi
  `Booking` chỉ được phép có tối đa 1 hóa đơn hợp lệ chưa thanh toán. Nếu hủy hóa đơn, phải tạo hóa đơn mới (điều chỉnh)
  chứ không được sửa trực tiếp.
- **BR-14**: Chỉ có `Employee` mới được phép cập nhật trạng thái `Invoice` từ `UNPAID` sang `PAID` (hoặc thực hiện hủy
  hóa đơn). Khách hàng không có quyền này.
- **BR-15**: `bookingCode` được sinh tự động theo định dạng `DS-{mã gồm chữ và số}` (VD: `DS-12AB5`). Mã độc nhất ở đuôi
  phải có ít nhất 5 ký tự.

### Entities

**Booking** (Thông tin đặt sân)

| Field       | Type                | Ghi chú                     |
|-------------|---------------------|-----------------------------|
| bookingId   | uuid                | PK                          |
| bookingCode | string              | UNIQUE                      |
| customerId  | uuid                | FK → Customer.customerId    |
| courtId     | int                 | FK → BadmintonCourt.courtId |
| date        | DateOnly            |                             |
| startTime   | TimeOnly            |                             |
| endTime     | TimeOnly            |                             |
| status      | Enum[BookingStatus] |                             |
| totalCost   | Decimal(10,2)       | snapshot theo BR-11         |
| createdAt   | DateTime            |                             |
| updatedAt   | DateTime            |                             |

**BookingStatusHistory** (Lịch sử thay đổi trạng thái đặt sân)

| Field             | Type                      | Ghi chú                                   |
|-------------------|---------------------------|-------------------------------------------|
| id                | uuid                      | PK                                        |
| bookingId         | uuid                      | FK → Booking.bookingId                    |
| oldStatus         | Enum[BookingStatus], NULL | NULL khi ghi log lần tạo Booking đầu tiên |
| newStatus         | Enum[BookingStatus]       |                                           |
| changedByEmployee | uuid, NULL                | FK → Employee.employeeId                  |
| changedAt         | DateTime                  |                                           |
| reason            | string                    |                                           |

**Invoice** (Hóa đơn)

| Field              | Type                      | Ghi chú                  |
|--------------------|---------------------------|--------------------------|
| invoiceId          | uuid                      | PK                       |
| invoiceCode        | string                    | UNIQUE                   |
| bookingId          | uuid                      | FK → Booking.bookingId   |
| totalCost          | Decimal(10,2)             |                          |
| status             | Enum[InvoiceStatus]       |                          |
| issuedByEmployee   | uuid                      | FK → Employee.employeeId |
| issuedAt           | DateTime                  |                          |
| paidAt             | DateTime, NULL            | NULL khi chưa thanh toán |
| paymentMethod      | Enum[PaymentMethod], NULL | NULL khi chưa thanh toán |
| note               | string                    |                          |

### Entity Constraints

**Booking**

```sql
UNIQUE (bookingCode)

-- enforce BR-09 (không trùng lịch, trừ Cancelled)
EXCLUDE USING gist (
  courtId WITH =,
  date WITH =,
  tsrange(date + startTime, date + endTime) WITH &&
) WHERE (status <> 'Cancelled')
```

**Invoice**

```sql
UNIQUE (invoiceCode)

-- enforce BR-13 (tối đa 1 hóa đơn UNPAID / booking)
CREATE UNIQUE INDEX ON Invoice (bookingId) WHERE status = 'Unpaid'
```

> EF Core lưu các enum booking/invoice bằng tên PascalCase (`Pending`, `Unpaid`, ...); các giá trị viết hoa bên dưới là contract nghiệp vụ/API.

### Enums

| Enum          | Values                                           |
|---------------|--------------------------------------------------|
| BookingStatus | `PENDING`, `CONFIRMED`, `COMPLETED`, `CANCELLED` |
| InvoiceStatus | `UNPAID`, `PAID`, `CANCELLED`                    |
| PaymentMethod | `BANK`, `CASH`                                   |

---

## 4. Hệ thống

### Business Rules

- **BR-16**: Mọi hành động thêm, sửa, xóa (CRUD) trên các bảng quan trọng — `BadmintonCourt`, `CourtSchedule`,
  `PricingRule`, `Booking`, `Invoice` — bắt buộc phải ghi vào bảng `Audit`.
- **BR-17**: Hành động đăng nhập thất bại quá 5 lần liên tiếp của một `UserAccount` phải bị khóa tài khoản tạm thời (cập
  nhật `status` trong `UserAccount`) và ghi log vào `Audit`.
- **BR-18**: Hệ thống **không** được phép xóa vật lý (DELETE) bất kỳ bản ghi nào có liên quan đến giao dịch (`Booking`,
  `Invoice`, `Customer` đã từng đặt sân). Thay vào đó, sử dụng trạng thái (Status) để đánh dấu đã xóa hoặc vô hiệu.

### Entities

**Audit** (Lịch sử sửa đổi của toàn hệ thống; được đánh dấu `NoAudit` để không tự ghi audit)

| Field      | Type            | Ghi chú                                  |
|------------|-----------------|------------------------------------------|
| id         | int             | PK                                       |
| actorType  | Enum[ActorType] | Loại tác nhân                            |
| employeeId | uuid, NULL      | FK → Employee.employeeId                 |
| customerId | uuid, NULL      | FK → Customer.customerId                 |
| action     | string          | Tối đa 20 ký tự                          |
| entityName | string          | Tối đa 100 ký tự                         |
| entityId   | string          | Tối đa 100 ký tự                         |
| oldValue   | JsonB, NULL     | Giá trị trước thay đổi                   |
| newValue   | JsonB, NULL     | Giá trị sau thay đổi                     |
| createdAt  | DateTime        | Thời điểm UTC                            |

### Entity Constraints

```sql
CHECK (employeeId IS NULL OR customerId IS NULL)
INDEX (entityName, entityId)
INDEX (createdAt)
INDEX (employeeId)
INDEX (customerId)
```

### Enums

| Enum      | Values                          |
|-----------|---------------------------------|
| ActorType | `SYSTEM`, `EMPLOYEE`, `CUSTOMER` |

> `ActorType` được EF Core lưu dưới dạng chuỗi PascalCase.