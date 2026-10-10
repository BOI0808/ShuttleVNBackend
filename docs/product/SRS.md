# Tài liệu đặc tả yêu cầu phần mềm (SRS)

**Hệ thống đặt sân cầu lông** · Phiên bản 1.5 · 10/10/2026

## 1. Giới thiệu

### 1.1 Mục đích

Tài liệu đặc tả yêu cầu chức năng, quy tắc nghiệp vụ và yêu cầu phi chức năng của Hệ thống đặt sân cầu lông, làm cơ sở để các bên thống nhất phạm vi, phát triển, kiểm thử và nghiệm thu.

### 1.2 Phạm vi

Hệ thống hỗ trợ cơ sở kinh doanh sân cầu lông quản lý sân, lịch mở cửa, bảng giá; tiếp nhận và xử lý đặt sân; lập hóa đơn; thống kê vận hành. Hệ thống phục vụ ba nhóm người dùng: khách hàng, nhân viên và quản trị viên.

Ngoài phạm vi phiên bản này:

- Kết nối cổng thanh toán trực tuyến, tự thu tiền và tự hoàn tiền. Hệ thống chỉ ghi nhận việc thu/hoàn tiền do nhân viên xác nhận; việc chuyển tiền thực hiện ngoài hệ thống. Không hỗ trợ thanh toán từng phần, giữ cọc hoặc hoàn tiền từng phần trong phiên bản này.
- Gọi điện trực tuyến (VoIP). Hotline chỉ hiển thị số điện thoại.
- Quản lý danh sách nhiều phiên đăng nhập, thiết bị và thu hồi từng/toàn bộ phiên. Hiện tại không thiết kế bảng Session hoặc trường phiên bản phiên; vẫn kiểm tra tài khoản tồn tại, Hoạt động và quyền hiện tại trên các yêu cầu cần đăng nhập.

### 1.3 Thuật ngữ

| Thuật ngữ | Định nghĩa |
| --- | --- |
| Khách thành viên | Khách hàng có tài khoản (hồ sơ Khách hàng liên kết với tài khoản) và đã đăng nhập. |
| Khách vãng lai | Người đặt nhanh khi chưa đăng nhập, dùng họ tên, số điện thoại và email. Hồ sơ có thể chưa có tài khoản hoặc đã liên kết tài khoản; việc đặt nhanh không cấp quyền truy cập tài khoản hay lịch sử của hồ sơ đó. |
| Nhân viên | Người dùng nội bộ có tài khoản do quản trị viên cấp, dùng hồ sơ Nhân viên. |
| Quản trị viên (QTV) | Người dùng nội bộ dùng hồ sơ Nhân viên và được cấp quyền quản trị. |
| Đặt sân | Một lượt giữ sân của một khách hàng theo ngày, giờ bắt đầu và giờ kết thúc. |
| Lưới sân (CourtGrid) | Màn hình hiển thị tình trạng từng sân theo khung giờ trong một ngày. |
| Slot | Một khoảng 30 phút trên lưới sân; thời gian bắt đầu và kết thúc đặt sân nằm ở phút 00 hoặc 30. |
| Mốc giá | Giờ bắt đầu áp dụng một đơn giá theo giờ; đơn giá có hiệu lực đến mốc kế tiếp hoặc giờ đóng cửa. |

### 1.4 Tài liệu tham chiếu

- Danh sách User stories (US-1 đến US-26). Cột Story trong mục 4 chỉ story nguồn của từng yêu cầu.
- Thiết kế dữ liệu của hệ thống (`database.md`).

## 2. Mô tả tổng quan

### 2.1 Nhóm người dùng

| Nhóm | Mô tả | Truy cập |
| --- | --- | --- |
| Khách vãng lai | Đặt nhanh khi chưa đăng nhập, xác minh email trước khi lưu. | Không đăng nhập |
| Khách thành viên | Đặt, hủy, đổi lịch và xem lịch sử đặt sân của mình. | Đăng ký, đăng nhập |
| Nhân viên | Xử lý đặt sân, quản lý hồ sơ và tài khoản khách hàng, hóa đơn, trạng thái sân. | Tài khoản do QTV cấp |
| Quản trị viên | Toàn bộ chức năng của nhân viên, cộng quản lý sân, giá, tài khoản, nhân sự, thống kê, nhật ký. | Tài khoản có quyền quản trị |

### 2.2 Phân quyền

| Chức năng | Khách vãng lai | Khách thành viên | Nhân viên | QTV |
| --- | :-: | :-: | :-: | :-: |
| Xem sân, lưới sân, giá | ✓ | ✓ | ✓ | ✓ |
| Trợ lý AI | ✓ | ✓ |  |  |
| Đặt sân | ✓ (đặt nhanh) | ✓ | ✓ (cho khách) | ✓ (cho khách) |
| Hủy, đổi lịch đặt sân | Qua nhân viên | Tự thực hiện khi chưa thanh toán; đã thanh toán qua nhân viên | ✓ | ✓ |
| Xem lịch sử đặt sân, hồ sơ cá nhân |  | ✓ (của mình) |  |  |
| Xử lý yêu cầu đặt sân, quản lý lịch đặt sân |  |  | ✓ | ✓ |
| Cập nhật trạng thái sân |  |  | ✓ | ✓ |
| Quản lý khách hàng, hóa đơn, tra cứu nhanh |  |  | ✓ | ✓ |
| Quản lý sân, lịch mở cửa, bảng giá |  |  |  | ✓ |
| Quản lý tài khoản khách hàng (tạo, cập nhật, khóa, mở khóa, đổi email) |  |  | ✓ | ✓ |
| Quản lý tài khoản nội bộ, phân quyền, nhân viên |  |  |  | ✓ |
| Theo dõi toàn hệ thống, thống kê, nhật ký |  |  |  | ✓ |

Ô trống nghĩa là không được cấp quyền. QTV kế thừa quyền nhân viên. Quyền của khách thành viên chỉ áp dụng cho hồ sơ và đặt sân liên kết với tài khoản đang đăng nhập; biết mã đặt sân hoặc email không tạo ra quyền truy cập.

### 2.3 Môi trường và ràng buộc

- Hệ thống là ứng dụng web, truy cập qua trình duyệt.
- Đơn vị tiền tệ: VND.
- Ngày trong tuần theo chuẩn ISO-8601: 1 = Thứ Hai, ..., 7 = Chủ Nhật.
- Ngày sử dụng sân và thời điểm chuyển sang ngày mới được xác định theo giờ Việt Nam (UTC+07:00); thời điểm lưu trữ vẫn theo UTC (NFR-07).
- Bộ lọc từ ngày/đến ngày gồm cả hai ngày theo giờ Việt Nam; máy chủ chuyển thành khoảng từ 00:00 ngày đầu (bao gồm) đến 00:00 ngày sau ngày cuối (không bao gồm) khi lọc thời điểm UTC. Từ ngày > đến ngày bị từ chối.
- Danh sách quản trị, lịch sử đặt sân và nhật ký có phân trang: mặc định 20 bản ghi, tối đa 100/trang; trang bắt đầu từ 1. Đồng thứ tự hiển thị dùng mã bản ghi làm tiêu chí ổn định.

## 3. Quy tắc nghiệp vụ

| Mã | Quy tắc |
| --- | --- |
| BR-01 | **Giờ mở cửa.** Mỗi sân có đúng một lịch cho mỗi ngày trong tuần, duy nhất theo cặp sân và ngày trong tuần. Lịch có một giờ mở, một giờ đóng (giờ mở < giờ đóng) và trạng thái khả dụng. Giờ mở/đóng ở phút 00 hoặc 30; không có nhiều khoảng mở cửa trong một ngày và không hỗ trợ lịch/đặt sân qua nửa đêm. Không xóa riêng lịch của một ngày; ngày nghỉ được đánh dấu không khả dụng. Đặt sân phải nằm trọn trong giờ mở cửa của ngày đó và lịch phải khả dụng. |
| BR-02 | **Không trùng lịch.** Một sân chỉ có tối đa một đặt sân chưa hủy tại cùng thời điểm. Khoảng sử dụng được tính từ giờ bắt đầu (bao gồm) đến giờ kết thúc (không bao gồm): 08:00–09:00 và 09:00–10:00 không trùng. Khi đổi lịch, loại chính đơn đang đổi khỏi phép kiểm tra. Kiểm tra khi tạo, đổi lịch, xác nhận và chuyển Hoàn thành cho đơn đã tồn tại; bảo đảm ở cơ sở dữ liệu và trong cùng giao dịch theo BR-21. |
| BR-03 | **Bảng giá theo mốc giờ.** Giá được cấu hình theo sân và ngày trong tuần bằng danh sách mốc giờ tăng dần, không trùng; đơn giá tại mỗi mốc phải là số nguyên đồng VND lớn hơn 0. Mốc đầu tiên bằng giờ mở cửa; mọi mốc phải từ giờ mở cửa đến trước giờ đóng cửa. Đơn giá tại một mốc áp dụng đến mốc kế tiếp; đơn giá tại mốc cuối áp dụng đến giờ đóng cửa. Bảng giá không lưu giờ kết thúc riêng và luôn phủ trọn giờ mở cửa. |
| BR-04 | **Tính tiền và lưu giá.** Máy chủ tính tổng chi phí theo số phút thực tế chịu từng đơn giá của BR-03, cộng các phần rồi làm tròn một lần đến đồng VND gần nhất (phần lẻ từ 0,5 đồng làm tròn lên). Tổng tiền và các mốc giá đã dùng được lưu tại lúc tạo; chỉ tính lại khi đổi sang sân/ngày/giờ khác. Không tính lại khi xác nhận hoặc chuyển Hoàn thành. Đổi bảng giá về sau không đổi giá đã lưu. Giá xem trước không giữ chỗ; nếu giá hoặc tổng tiền thay đổi trước khi gửi/lưu đơn, hệ thống trả báo giá mới để người thao tác xác nhận lại, không âm thầm lưu với số tiền khác. Ví dụ 16:30–18:00, giá trước 17:00 là 60.000 đ/giờ, sau 17:00 là 100.000 đ/giờ: 30 × 60.000/60 + 60 × 100.000/60 = 130.000 đ. |
| BR-05 | **Hủy và đổi lịch theo yêu cầu khách.** Chỉ với Chờ xác nhận hoặc Đã xác nhận, khi thời điểm máy chủ ≤ thời điểm bắt đầu hiện tại − 1 giờ; đúng mốc 1 giờ vẫn được xử lý. Khi đổi lịch, thời điểm bắt đầu mới cũng phải cách thời điểm xử lý ít nhất 1 giờ và thỏa BR-01, BR-02, BR-20. Đơn chưa thanh toán: khách thành viên tự xử lý, khách vãng lai qua nhân viên. Đơn đã thanh toán: chỉ nhân viên xử lý theo BR-23. Quy tắc này áp dụng cả khi nhân viên làm thay khách; hủy vì sự cố vận hành áp dụng BR-24. |
| BR-06 | **Vòng đời đặt sân.** Chờ xác nhận (`PENDING`): yêu cầu chưa được nhân viên xác nhận. Đã xác nhận (`CONFIRMED`): nhân viên đã xác nhận đặt sân, không phụ thuộc việc đã thanh toán hay chưa. Hoàn thành (`COMPLETED`): khách đã sử dụng sân, không đồng nghĩa với đã thanh toán. Đã hủy (`CANCELLED`): đặt sân bị hủy. Chuyển trạng thái hợp lệ: Chờ xác nhận → Đã xác nhận → Hoàn thành; Chờ xác nhận → Hoàn thành khi nhân viên ghi nhận khách đã sử dụng lượt sân của đơn đã tồn tại; Chờ xác nhận hoặc Đã xác nhận → Đã hủy. Hệ thống còn tự xử lý khi qua ngày sử dụng theo BR-17. Hoàn thành và Đã hủy là trạng thái cuối. |
| BR-07 | **Mã đặt sân.** Tự sinh theo định dạng DS-{chữ và số}, phần chữ và số có ít nhất 5 ký tự, không trùng nhau. Ví dụ: DS-12AB5. |
| BR-08 | **Lập hóa đơn tự động.** Chờ xác nhận chưa có hóa đơn. Nhân viên xác nhận thì chuyển đơn sang Đã xác nhận và tạo đúng một hóa đơn trong cùng giao dịch: Chưa thanh toán nếu chưa thu tiền, Đã thanh toán nếu đã thu đủ tổng tiền và ghi phương thức/thời điểm thu. Chuyển đơn đã tồn tại từ Chờ xác nhận → Hoàn thành cũng lập hóa đơn tương tự. Đã xác nhận → Hoàn thành dùng hóa đơn hiện có, không lập thêm và không tự thay đổi trạng thái thanh toán. Tổng tiền hóa đơn hợp lệ phải bằng tổng tiền đặt sân; thay đổi/điều chỉnh theo BR-22, BR-23. |
| BR-09 | **Hóa đơn.** Mã HD-{yyyyMMdd}-{số thứ tự}, dùng ngày lập theo giờ Việt Nam; số thứ tự tăng trong ngày, không tái sử dụng sau hủy và duy nhất kể cả khi lập đồng thời. Mỗi đơn Đã xác nhận hoặc Hoàn thành có đúng một hóa đơn hợp lệ (Chưa thanh toán hoặc Đã thanh toán); các hóa đơn Đã hủy chỉ dùng tra cứu. Chưa thanh toán → Đã thanh toán khi thu đủ tiền; Chưa thanh toán → Đã hủy khi hủy đơn hoặc lập bản thay thế. Hủy hóa đơn Đã thanh toán phải thỏa BR-23; không được tự hủy hóa đơn riêng lẻ rồi để đơn đang hiệu lực thiếu hóa đơn. Hóa đơn Đã hủy không kích hoạt lại hoặc nhận thanh toán. Nội dung hóa đơn bất biến sau khi lập; được đổi trạng thái, bổ sung thông tin thu/hoàn/hủy theo quy tắc. Thanh toán dùng Tiền mặt hoặc Chuyển khoản; Chưa thanh toán để trống phương thức và thời điểm thu, Đã thanh toán phải có đủ hai thông tin. Khi hủy hóa đơn đã thu tiền, giữ lại thông tin thu cũ và lưu thêm lý do, người/thời điểm hủy, thông tin hoàn tiền. |
| BR-10 | **Khách hàng và đặt nhanh.** Mỗi đặt sân thuộc đúng một hồ sơ Khách hàng; mỗi email chuẩn hóa chỉ có một hồ sơ. Đặt nhanh bằng email cũ tái sử dụng hồ sơ đó; email mới tạo hồ sơ chưa liên kết tài khoản. Trước khi lưu đặt nhanh, người đặt phải xác minh quyền dùng email bằng mã theo BR-26, kể cả email đã có tài khoản; không cần tạo tài khoản. Form đặt nhanh không ghi đè hồ sơ cũ và không trả tên, điện thoại, mã hồ sơ hoặc lịch sử cũ. Tài khoản nhân viên/QTV không sở hữu đặt sân; họ tạo thay khách sau khi kiểm tra thông tin liên hệ, tái sử dụng hồ sơ nếu email đã có. |
| BR-11 | **Tài khoản và tái sử dụng hồ sơ.** Mỗi tài khoản liên kết đúng một hồ sơ Nhân viên hoặc Khách hàng, không đồng thời cả hai; mỗi hồ sơ liên kết tối đa một tài khoản. Tài khoản nội bộ chỉ do QTV tạo. Tài khoản khách do khách tự đăng ký hoặc nhân viên tạo sau khi xác minh thông tin khách. Email đăng ký có hồ sơ chưa liên kết thì dùng lại mã khách hàng và lịch sử; không tạo hồ sơ trùng, không ghi đè thông tin cũ từ form đăng ký. Khách tự đăng ký phải xác minh email trước khi liên kết; nếu email đã dùng làm email đăng nhập hoặc hồ sơ đã có tài khoản, từ chối tài khoản thứ hai, hướng dẫn đăng nhập/quên mật khẩu. Tạo đồng thời phải bảo đảm duy nhất; yêu cầu thua xung đột không được để lại tài khoản/hồ sơ mồ côi. |
| BR-12 | **Khóa đăng nhập.** Khóa và vô hiệu hóa là cùng một trạng thái `DISABLED`; tài khoản chỉ có `ACTIVE` hoặc `DISABLED`. Sau 5 lần đăng nhập sai liên tiếp, tài khoản chuyển DISABLED ngay ở lần sai thứ 5 và ghi nhật ký. Không tự mở theo thời gian hoặc khi đặt lại mật khẩu. Nhân viên/QTV mở tài khoản khách; chỉ QTV mở tài khoản nội bộ. Mở khóa chuyển DISABLED → ACTIVE và đưa bộ đếm sai về 0; đăng nhập thành công cũng đưa bộ đếm về 0. Bộ đếm chỉ áp dụng cho sai mật khẩu của tài khoản ACTIVE, tách khỏi sai mã xác minh. Máy chủ kiểm tra tài khoản còn tồn tại, ACTIVE và quyền hiện tại trên mỗi yêu cầu cần đăng nhập; tài khoản DISABLED hoặc đã bị xóa không được tiếp tục dùng chức năng cần đăng nhập. Khách bị khóa vẫn có thể đặt nhanh sau xác minh email và làm việc qua nhân viên; xác minh đặt nhanh không cấp quyền truy cập tài khoản/lịch sử. |
| BR-13 | **Lịch sử trạng thái.** Tạo đơn ghi trạng thái cũ trống và trạng thái khởi tạo; mỗi lần đổi trạng thái ghi cũ/mới, tác nhân (khách hàng, nhân viên hoặc hệ thống), mã người thực hiện khi có, thời điểm và lý do. Ghi lịch sử trong cùng giao dịch với thay đổi; không dùng mã nhân viên trống để suy ra hệ thống vì khách cũng tự hủy. Lịch sử chỉ đọc; lặp lại yêu cầu đã thành công không ghi thêm một lần chuyển trạng thái. |
| BR-14 | **Nhật ký.** Ghi mọi thao tác thêm/sửa/xóa theo quy tắc của từng đối tượng: sân, lịch, giá, hồ sơ khách, tài khoản, đặt sân, hóa đơn; gồm tác nhân, đối tượng, giá trị trước/sau, thời điểm và lý do khi cần. Ghi cả hard-delete tài khoản, khóa/mở khóa, đổi email/phân quyền, thu/hoàn/hủy hóa đơn, tác vụ tự xử lý. Nhật ký xóa tài khoản giữ mã tài khoản và thông tin được phép trong giá trị trước, không phụ thuộc tài khoản đích còn tồn tại. Không ghi mật khẩu, mã xác minh, mã phiên hoặc các giá trị băm của chúng. Nhật ký không sửa/xóa qua ứng dụng. |
| BR-15 | **Xóa tài khoản và bảo toàn lịch sử.** Xóa tài khoản là hard-delete bản ghi tài khoản. Không cascade xóa hồ sơ Nhân viên/Khách hàng, đặt sân, hóa đơn hoặc nhật ký; liên kết accountId trên hồ sơ được đặt NULL trong cùng giao dịch. Giữ nguyên hồ sơ, mã khách/nhân viên và dữ liệu giao dịch để bảo toàn liên kết lịch sử. Đặt sân/hóa đơn hủy bằng trạng thái; sân xóa mềm theo BR-19. Không có chức năng xóa hồ sơ khách đã giao dịch trong phiên bản này. Lịch và bảng giá hiện hành có thể thay thế theo BR-03, BR-25 nhưng nhật ký và giá đã lưu của giao dịch không bị xóa theo. Xóa tài khoản không hủy đặt sân/hóa đơn, không tương đương khóa tài khoản và không có thao tác mở khóa để khôi phục bản ghi đã xóa. |
| BR-16 | **Bảo vệ lịch và trạng thái sân.** Từ chối thay đổi lịch nếu một đơn Chờ xác nhận/Đã xác nhận gắn với ngày bị ảnh hưởng sẽ nằm ngoài giờ mở mới hoặc trở thành không khả dụng. Mở rộng giờ hoặc thay đổi không ảnh hưởng các đơn này được phép. Chuyển sân sang Bảo trì/Đóng cũng phải xử lý hết đơn Chờ xác nhận/Đã xác nhận còn gắn với sân; dùng BR-24 để xử lý sự cố. Thay đổi và kiểm tra thực hiện đồng thời với việc bảo vệ chống tạo/xác nhận/đổi đơn trong khoảng đang sửa (BR-21). Sửa lịch có hiệu lực từ ngày hiện tại, không sửa lịch sử các ngày đã qua; thay đổi trong ngày hiện tại cũng không được loại khỏi giờ mở các lượt Hoàn thành đã ghi nhận trong ngày. |
| BR-17 | **Tự xử lý qua ngày sử dụng.** Tại 00:00 giờ Việt Nam, xử lý mọi đơn có ngày sử dụng < ngày hiện tại và còn Chờ xác nhận/Đã xác nhận: có hóa đơn hợp lệ Đã thanh toán thì chuyển Hoàn thành; còn Chưa thanh toán hoặc không có hóa đơn hợp lệ thì chuyển Đã hủy, đồng thời hủy hóa đơn Chưa thanh toán nếu có. Đây là quy tắc chốt tự động theo ngày; không phải bằng chứng khách có mặt. Đơn đã Hoàn thành/Đã hủy giữ nguyên, kể cả Hoàn thành chưa trả tiền. Nếu tác vụ gián đoạn, chạy bù các ngày bỏ lỡ khi hoạt động lại. Trước mọi thao tác làm thay đổi một đơn quá ngày chưa được chốt, máy chủ phải áp dụng quy tắc này; không nhận thanh toán hoặc đổi/xác nhận để cứu đơn lẽ ra bị tự hủy. Thanh toán sát mốc 00:00 và chốt tự động phải được quyết định theo thứ tự giao dịch và thời điểm xử lý của máy chủ (BR-21). |
| BR-18 | **Đổi email khách hàng.** Khách không tự đổi email qua giao diện/API. Nhân viên/QTV thực hiện sau khi kiểm tra danh tính khách và quyền dùng email mới theo BR-26. Email mới được chuẩn hóa, không trùng hồ sơ hoặc email đăng nhập khác; trùng chính email hiện tại là không thay đổi. Cập nhật đồng thời email hồ sơ và email đăng nhập nếu có tài khoản; giữ mã khách, tài khoản và lịch sử, không gộp hai hồ sơ. Vô hiệu mã xác minh của email cũ trong cùng thao tác. Ghi nhật ký; không xác thực quyền sở hữu chỉ bằng việc người yêu cầu biết email cũ. |
| BR-19 | **Xóa mềm sân và giải phóng tên.** QTV chuyển sân sang Đã xóa (`DELETED`) và tên thành `NULL` trong cùng giao dịch; không dùng chuỗi rỗng. Tên sau khi bỏ khoảng trắng đầu/cuối bắt buộc, không trùng nếu so sánh không phân biệt hoa/thường; chỉ sân Đã xóa được có tên NULL. Lưu tên cũ trong nhật ký; cho phép sân mới dùng lại tên. Giữ mã sân, lịch, giá và giao dịch cũ; màn hình lịch sử dùng nhãn “Sân đã xóa #<mã sân>”. Đã xóa không hiển thị trên lưới/danh sách đặt mới, không sửa hoặc khôi phục. Trước khi xóa phải xử lý hết đơn Chờ xác nhận/Đã xác nhận; kiểm tra và xóa được bảo vệ khỏi việc tạo/đổi/xác nhận đơn đồng thời theo BR-21. Xóa lặp lại trả kết quả Đã xóa, không ghi thêm nhật ký thay đổi. |
| BR-20 | **Ngày giờ đặt sân.** Mọi giờ có độ chính xác đến phút; giờ bắt đầu/kết thúc ở phút 00 hoặc 30, bắt đầu < kết thúc, cùng một ngày; thời lượng tối thiểu 30 phút. Mọi đơn mới, kể cả nhân viên tạo thay khách, phải có bắt đầu > thời điểm máy chủ và khởi tạo ở Chờ xác nhận. Không tạo đơn mới cho lượt đã bắt đầu hoặc đã kết thúc, không tạo trực tiếp Hoàn thành. Xác nhận chỉ trước giờ bắt đầu và sân còn Hoạt động/khả dụng; sau giờ bắt đầu chỉ được ghi nhận sử dụng cho đơn đã tồn tại hoặc hủy vì sự cố. Hoàn thành thủ công chỉ khi nhân viên xác nhận khách đã sử dụng lượt sân của đơn, giờ kết thúc ≤ thời điểm xử lý và đơn còn Chờ xác nhận/Đã xác nhận sau khi áp dụng BR-17 nếu cần. Giữ lịch và giá đã lưu trên đơn; không tính lại theo bảng giá hiện tại. Không hồi sinh đơn Đã hủy. |
| BR-21 | **Đồng thời, nguyên tử và gửi lại.** Một thao tác nghiệp vụ phải lưu tất cả thay đổi liên quan (hồ sơ/tài khoản, đặt sân, hóa đơn, lịch sử, nhật ký) trong một giao dịch; lỗi bất kỳ bước nào thì hoàn tác toàn bộ. Kiểm tra lại quyền, trạng thái, thời gian, giá và ràng buộc duy nhất ngay khi thực thi dưới cơ chế chống xung đột. Hai yêu cầu cùng đặt một chỗ chỉ một thành công; hai yêu cầu sửa cùng bản ghi phải kiểm tra lại dữ liệu gốc dưới cơ chế chống xung đột, yêu cầu dựa trên dữ liệu đã thay đổi bị từ chối và phải tải lại. Mỗi yêu cầu tạo/đổi/xác nhận/hủy/thu tiền có mã chống gửi lặp gắn với người thao tác: cùng mã và cùng nội dung trả kết quả cũ, cùng mã khác nội dung bị từ chối; lưu mã cùng giao dịch, giữ suốt vòng đời bản ghi. Tác vụ tự xử lý cũng không tạo lặp hóa đơn/lịch sử. Thời điểm quyết định là thời điểm máy chủ sau khi giành quyền xử lý bản ghi, không dùng đồng hồ trình duyệt hoặc thời điểm nhấn nút. |
| BR-22 | **Đổi/hủy đơn chưa thanh toán và điều chỉnh hóa đơn.** Hủy đơn chưa thu tiền đồng thời hủy hóa đơn Chưa thanh toán nếu có. Đổi đơn Chờ xác nhận vẫn Chờ xác nhận, chưa lập hóa đơn. Đổi đơn Đã xác nhận vẫn Đã xác nhận; tính giá mới và hủy hóa đơn Chưa thanh toán cũ, lập một hóa đơn Chưa thanh toán thay thế cùng tổng tiền mới trong một giao dịch. Không thể giữ hóa đơn cũ khác tổng tiền đơn. Nhân viên điều chỉnh hóa đơn phải nêu lý do và mã hóa đơn thay thế; không được tự nhập tổng tiền khác giá đã lưu của đặt sân. Sửa phương thức/thời điểm đã thu hoặc giá của lượt đã Hoàn thành không nằm trong chức năng điều chỉnh nội dung của phiên bản này. |
| BR-23 | **Đơn đã thanh toán và hoàn tiền.** Hủy/đổi do nhân viên thực hiện và vẫn thỏa BR-05 nếu theo yêu cầu khách. Hủy đơn đã thu tiền hoặc đổi làm tổng tiền khác phải hoàn toàn bộ khoản đã thu ngoài hệ thống; nhân viên xác nhận đã hoàn, lưu số tiền bằng tổng hóa đơn cũ, phương thức, thời điểm thực tế, người thực hiện và lý do trước khi hủy hóa đơn cũ. Hủy đơn: không lập hóa đơn thay thế. Đổi lịch: hủy hóa đơn cũ và lập hóa đơn mới theo giá mới; mới là Chưa thanh toán, hoặc Đã thanh toán khi đã thu đủ khoản mới và ghi thông tin thu mới. Không chuyển tiền cũ sang hóa đơn mới hoặc ghi thu lần hai nếu chưa thực thu. Đổi lịch có cùng tổng tiền giữ hóa đơn Đã thanh toán hiện có, không cần hoàn/thu lại. Toàn bộ ghi nhận thực hiện theo BR-21; mã chống gửi lặp không thay thế kiểm tra thực tế hoàn/thu tiền của nhân viên. Không hỗ trợ hoàn một phần; không coi việc đổi trạng thái hóa đơn là đã tự chuyển tiền. Kiểm tra điều kiện/giá/dữ liệu hiện tại trước khi nhân viên thực hiện thu/hoàn bên ngoài. Tiền đã chuyển ngoài hệ thống không thể hoàn tác bằng giao dịch dữ liệu; nếu ghi nhận lỗi sau khi đã chuyển, chỉ thử lại bước ghi nhận với chứng cứ giao dịch cũ, đối chiếu lại trạng thái và chuyển nhân viên xử lý khi có xung đột, không yêu cầu chuyển tiền lần nữa. |
| BR-24 | **Hủy do sự cố vận hành.** Nhân viên/QTV được hủy Chờ xác nhận/Đã xác nhận vì sân không thể phục vụ dù đã qua mốc 1 giờ, phải nêu lý do sự cố và xác nhận khách chưa sử dụng lượt sân. Nếu khách đã sử dụng, ghi Hoàn thành; không hủy để xóa lượt sử dụng. Đồng bộ hóa đơn theo BR-22/BR-23, thông báo khách; không thay đổi đơn đã ở trạng thái cuối. Muốn chuyển sân sang Bảo trì/Đóng/Đã xóa phải hoàn tất việc hủy hoặc đổi các đơn còn hiệu lực trước, không tự bỏ qua hoặc tự xóa chúng. |
| BR-25 | **Giờ mở cửa và bảng giá khi cấu hình.** Tạo sân yêu cầu QTV nhập lịch mặc định (giờ mở/đóng) và đơn giá mặc định > 0; hệ thống tạo 7 lịch cùng cấu hình và một mốc giá bằng giờ mở cho mỗi ngày, sau đó cho sửa từng ngày. Ngày không khả dụng vẫn giữ lịch và bảng giá hợp lệ. Khi đổi giờ mở/đóng, tự bỏ các mốc ngoài khoảng mới; thêm mốc tại giờ mở mới bằng đơn giá cũ có hiệu lực tại đó, hoặc đơn giá mốc cũ đầu tiên nếu mở sớm hơn. Giữ các mốc nằm sau giờ mở mới và trước giờ đóng mới; mốc cuối kéo dài đến giờ đóng. Xem trước và xác nhận cấu hình kết quả; lưu lịch/giá cùng giao dịch, ghi nhật ký, không để sân thiếu giá. Bảng giá do QTV nhập phải hợp lệ toàn bộ; không tự sửa dữ liệu giá không hợp lệ hoặc chấp nhận bảng rỗng. Giá trên đơn đã tạo được bảo toàn theo BR-04; không yêu cầu lưu phiên bản bảng giá để tính lại giao dịch quá khứ. |
| BR-26 | **Email, mật khẩu và mã xác minh.** Email bỏ khoảng trắng đầu/cuối, chuẩn hóa chữ thường để so sánh duy nhất và phải đúng định dạng; không tự gộp alias, bỏ dấu chấm hay phần sau dấu cộng. Mật khẩu dài 8–128 ký tự, không cắt ngắn hoặc tự bỏ khoảng trắng. Mã gồm 6 chữ số, hết hạn sau 10 phút, dùng một lần và tối đa 5 lần sai; lần sai thứ 5 vô hiệu mã, tách biệt bộ đếm đăng nhập. Gửi lại sớm nhất sau 60 giây và vô hiệu mã cũ cùng mục đích; một email/mục đích tối đa 5 lần gửi trong cửa sổ 1 giờ tính lùi từ thời điểm yêu cầu; mỗi nguồn truy cập tối đa 30 yêu cầu gửi mã trong cùng cửa sổ 1 giờ; vượt giới hạn trả yêu cầu thử lại sau, không khóa tài khoản đăng nhập. Khi tạo hồ sơ/đặt nhanh, các đầu vào họ tên/số điện thoại/email bắt buộc, họ tên không chỉ có khoảng trắng; số điện thoại sau khi bỏ dấu cách/dấu gạch ngang cho phép dấu + ở đầu và từ 8 đến 15 chữ số, giữ dạng chuỗi, không ép số nguyên. Không dùng số điện thoại để tự gộp hồ sơ. Xác minh gắn với email và mục đích đăng ký, đặt lại mật khẩu, đặt nhanh hoặc đổi email, không dùng chéo. Đặt nhanh: mã còn gắn với nội dung yêu cầu đặt, chỉ lưu đơn và tiêu thụ mã trong cùng giao dịch; lỗi lưu không tiêu thụ mã. Đặt lại mật khẩu/đổi email không tự mở tài khoản DISABLED; quản lý và thu hồi danh sách phiên không thuộc phạm vi hiện tại. Tra cứu/gửi mã công khai không trả hồ sơ cũ hoặc tiết lộ tài khoản tồn tại; lỗi gửi email không tạo đơn/tài khoản hoặc báo đã gửi thành công. Đổi email mới trùng hồ sơ khác không gộp hồ sơ; khách có thể tiếp tục dùng email cũ. |
| BR-27 | **Phân quyền và quản trị tài khoản.** Nhân viên chỉ quản lý tài khoản khách, được khóa/mở khóa theo BR-12; không hard-delete tài khoản, đổi loại tài khoản hoặc cấp quyền nội bộ. QTV quản lý mọi tài khoản, thực hiện hard-delete theo BR-15 và thay quyền Nhân viên ↔ QTV trên hồ sơ Nhân viên; không chuyển tài khoản khách thành nhân viên hoặc ngược lại. Không cho thao tác khóa/xóa/hạ quyền làm mất QTV ACTIVE cuối cùng; chỉ tính QTV còn tài khoản ACTIVE. Khóa tự động ở lần sai thứ 5 vẫn áp dụng cho QTV cuối; khi không còn QTV đăng nhập được, phục hồi bằng quy trình vận hành có người có thẩm quyền mở khóa, xác minh danh tính và lưu dấu vết, không tự mở theo thời gian. Máy chủ kiểm tra quyền hiện tại, không tiếp tục dùng quyền cũ sau thay đổi. Khóa hoặc xóa tài khoản không hủy đặt sân/hóa đơn. Tài khoản QTV đầu tiên cấp qua quy trình triển khai, không tự đăng ký công khai; không tạo thông tin đăng nhập mặc định trong tài liệu. Hồ sơ nhân viên khi tạo phải gắn tài khoản nội bộ, có email và đồng bộ email đăng nhập; sau hard-delete tài khoản thì accountId NULL và giữ hồ sơ lịch sử. Số điện thoại không là khóa duy nhất. |
| BR-28 | **Hiển thị, thông báo và AI.** Lưới công khai chỉ thể hiện khả dụng, đơn giá và bận/trống, không lộ người đặt hoặc thông tin hóa đơn. Nhân viên được xem dữ liệu phục vụ xử lý; khách đã đăng nhập chỉ xem hồ sơ/đơn của mình. Khi tạo hoặc đổi trạng thái, gửi email đến khách sau khi giao dịch thành công; kết quả nghiệp vụ vẫn giữ nếu gửi thất bại, ghi lỗi và gửi lại, không tạo lại đơn/hóa đơn để gửi email. Trợ lý AI chỉ đọc thông tin công khai để tư vấn/gợi ý; không tự đặt/giữ/hủy/đổi sân, thu tiền, mở khóa hoặc đọc dữ liệu cá nhân. Giá và chỗ trống AI gợi ý phải kiểm tra lại khi đặt; AI không có dữ liệu phù hợp thì thông báo và hướng khách đến nhân viên. |

### 3.1 Chuyển trạng thái đặt sân

Thao tác gửi lặp có cùng mã yêu cầu được xử lý theo BR-21 trước khi thực hiện một chuyển trạng thái mới. Hoàn thành thủ công độc lập với thanh toán; Hoàn thành tự động chỉ áp dụng điều kiện BR-17.

| Trạng thái hiện tại | Thao tác | Điều kiện | Kết quả |
| --- | --- | --- | --- |
| Chưa có đơn | Khách/nhân viên tạo trước lượt chơi | BR-01, BR-02, BR-04, BR-10, BR-20 | Chờ xác nhận; chưa có hóa đơn |
| Chờ xác nhận | Nhân viên xác nhận | Chưa đến giờ bắt đầu; sân Hoạt động, lịch khả dụng; không trùng | Đã xác nhận; lập hóa đơn Chưa thanh toán/Đã thanh toán |
| Chờ xác nhận | Nhân viên từ chối | Có lý do; đơn chưa quá ngày cần chốt | Đã hủy; không lập hóa đơn |
| Chờ xác nhận hoặc Đã xác nhận | Hủy theo yêu cầu khách | BR-05; đã thanh toán phải qua nhân viên, BR-23 | Đã hủy; hủy hóa đơn nếu có |
| Chờ xác nhận hoặc Đã xác nhận | Đổi lịch | BR-05; lịch/giá mới hợp lệ; xử lý hóa đơn BR-22/BR-23 | Giữ trạng thái, mã đơn, chủ hồ sơ; đổi lịch/giá |
| Chờ xác nhận hoặc Đã xác nhận | Nhân viên ghi Hoàn thành | Lượt thực tế đã kết thúc; BR-20 | Hoàn thành; lập hóa đơn nếu Chờ xác nhận, giữ hóa đơn nếu Đã xác nhận |
| Chờ xác nhận hoặc Đã xác nhận | Nhân viên hủy sự cố | BR-24; khách chưa sử dụng lượt sân | Đã hủy; đồng bộ hóa đơn/hoàn tiền |
| Chờ xác nhận hoặc Đã xác nhận | Chốt ngày | Ngày sử dụng đã qua, có hóa đơn hợp lệ Đã thanh toán | Hoàn thành tự động; giữ hóa đơn |
| Chờ xác nhận hoặc Đã xác nhận | Chốt ngày | Ngày sử dụng đã qua, chưa thanh toán | Đã hủy; hủy hóa đơn Chưa thanh toán nếu có |
| Hoàn thành | Thu tiền còn thiếu toàn bộ | Hóa đơn hiện có Chưa thanh toán, thu đủ tổng tiền | Giữ Hoàn thành; hóa đơn Đã thanh toán |
| Hoàn thành hoặc Đã hủy | Đổi lịch/xác nhận/chuyển sang trạng thái khác | Không cho phép, kể cả QTV | Giữ trạng thái cuối; trả lỗi nghiệp vụ |

### 3.2 Thứ tự kiểm tra và kết quả lỗi

Áp dụng chung cho khách và nhân viên; không trả lỗi làm lộ dữ liệu trước khi xác định quyền. Thời gian được xác định theo BR-21.

| Thứ tự | Kiểm tra | Kết quả nếu không đạt |
| --- | --- | --- |
| 1 | Phiên và quyền vai trò; quyền sở hữu đơn/hồ sơ nếu là khách | Yêu cầu đăng nhập hoặc từ chối truy cập; truy cập mã đơn của người khác trả Không tìm thấy, không kèm thông tin đơn |
| 2 | Cú pháp dữ liệu, mã yêu cầu gửi lặp và kết quả cũ nếu có | Nêu trường không hợp lệ; cùng mã khác nội dung bị từ chối; cùng mã cùng nội dung trả kết quả đã lưu sau khi kiểm tra quyền |
| 3 | Bản ghi tồn tại, dữ liệu gốc và chốt các đơn quá ngày theo BR-17 | Không tìm thấy hoặc Dữ liệu đã thay đổi; chốt quá ngày trước khi tiếp tục. Chốt là giao dịch hệ thống độc lập, không bị hoàn tác chỉ vì thao tác người dùng tiếp theo bị từ chối |
| 4 | Thao tác đã hoàn tất trên trạng thái hiện tại chưa | Hủy lại Đã hủy, xác nhận lại Đã xác nhận, Hoàn thành lại Hoàn thành, thu lại với cùng nội dung: trả kết quả hiện có, không đổi dữ liệu/lịch sử; phải đối chiếu định danh hóa đơn và thông tin thu, không áp dụng cho yêu cầu sửa nội dung khác |
| 5 | Chuyển trạng thái có hợp lệ không | Trạng thái không cho phép thao tác; đơn Đã hủy không được thu/xác nhận/Hoàn thành |
| 6 | Mốc giờ thao tác và điều kiện sử dụng thực tế | Quá hạn hủy/đổi, chưa kết thúc lượt, đã đến giờ bắt đầu hoặc cần xử lý qua nhân viên |
| 7 | Sân chưa xóa, Hoạt động khi cần; lịch/giờ mới khả dụng và đúng quy tắc | Sân/lịch không khả dụng hoặc giờ không hợp lệ |
| 8 | Không trùng lịch, tổng tiền xem trước, tình trạng hóa đơn và xử lý tiền | Slot đã được đặt, cần xác nhận báo giá mới hoặc chưa hoàn tất xử lý tiền; giữ nguyên dữ liệu của thao tác thất bại |
| 9 | Ràng buộc duy nhất/chống xung đột ngay khi lưu và ghi đầy đủ lịch sử/nhật ký | Nếu xung đột, hoàn tác thao tác đang lưu, trả dữ liệu cần tải lại; không báo thành công một phần |

Không thể bảo đảm lỗi ở bước 8 luôn được phát hiện trước bước 9 khi có thao tác cạnh tranh; bước 9 là kiểm tra cuối cùng bắt buộc. Sau khi đã chốt quá ngày, nếu đơn được Hoàn thành tự động thì yêu cầu xác nhận cũ không được áp lại để ghi thêm sự kiện.

### 3.3 Cách đọc trạng thái thanh toán

| Trạng thái đặt sân | Hóa đơn | Hiển thị và thao tác |
| --- | --- | --- |
| Chờ xác nhận | Chưa có | Hiển thị Chưa lập hóa đơn, không ghi Đã thanh toán |
| Đã xác nhận | Chưa thanh toán hoặc Đã thanh toán | Tách nhãn xác nhận đặt sân và nhãn thanh toán; có đúng một hóa đơn hợp lệ |
| Hoàn thành | Chưa thanh toán | Khách đã dùng hoặc được nhân viên ghi nhận dùng; vẫn còn khoản cần thu. Không tự hủy vào ngày sau |
| Hoàn thành | Đã thanh toán | Hoàn thành và đã thu đủ; xem lịch sử để phân biệt thủ công hay chốt tự động |
| Đã hủy | Chưa có hoặc các hóa đơn Đã hủy | Không nhận tiền hoặc lập hóa đơn mới; thông tin thu/hoàn cũ chỉ để tra cứu |

## 4. Yêu cầu chức năng

### 4.1 Chức năng khách hàng

| Mã | Yêu cầu | Story |
| --- | --- | --- |
| KH-01 | Cho phép khách đăng ký tài khoản bằng họ tên, số điện thoại, email và mật khẩu theo BR-26. Email là tên đăng nhập và không được trùng với tài khoản khác. Nếu đã có hồ sơ khách vãng lai cùng email, liên kết tài khoản mới với hồ sơ đó theo BR-11. | US-1 |
| KH-02 | Gửi mã xác minh email đăng ký và xử lý hết hạn/gửi lại/nhập sai theo BR-26. Chỉ tạo tài khoản và liên kết hồ sơ sau khi xác minh thành công. | US-1 |
| KH-03 | Đăng nhập bằng email và mật khẩu. Tài khoản DISABLED (bị khóa/vô hiệu hóa) hoặc đã bị xóa không đăng nhập được. | US-1 |
| KH-04 | Quên mật khẩu: xác minh email để đặt mật khẩu mới theo BR-26; không tự mở tài khoản DISABLED. | US-1 |
| KH-05 | Hiển thị danh sách sân gồm tên, mô tả và trạng thái, không yêu cầu đăng nhập; không hiển thị sân Đã xóa. | US-2 |
| KH-06 | Hiển thị lưới theo ngày, mỗi slot 30 phút trong giờ mở cửa. Chờ xác nhận/Đã xác nhận/Hoàn thành giữ chỗ, Đã hủy không giữ chỗ. Slot trống màu xanh và có nhãn; slot bận hoặc sân/lịch không khả dụng được phân biệt bằng nhãn/trạng thái, không chỉ màu. Không hiển thị sân Đã xóa hoặc thông tin cá nhân người đặt. | US-2 |
| KH-07 | Hiển thị đơn giá theo khung giờ của từng sân. | US-2 |
| KH-08 | Khách thành viên chọn sân, ngày, giờ bắt đầu/kết thúc theo BR-20. Máy chủ trả tổng tiền xem trước theo BR-04; xem giá hoặc chọn slot chưa giữ chỗ. | US-3 |
| KH-09 | Đặt sân thành công tạo đặt sân ở trạng thái Chờ xác nhận, có mã đặt sân (BR-07) hiển thị cho khách và lưu trong lịch sử đặt sân. | US-3 |
| KH-10 | Từ chối đặt sân và nêu lý do nếu vi phạm BR-01, BR-02, BR-20, sân không Hoạt động, hồ sơ/tài khoản không hợp lệ hoặc báo giá đã đổi; xung đột không tạo đơn một phần. | US-3 |
| KH-11 | Đặt nhanh bằng họ tên, số điện thoại, email và sân/ngày/giờ; không cần tài khoản. Xác minh email theo BR-26 trước khi lưu và tái sử dụng hồ sơ theo BR-10. Thành công trả mã đặt sân mới, không trả dữ liệu cũ của hồ sơ. | US-4 |
| KH-12 | Tra cứu hồ sơ khách hàng theo email: tái sử dụng hồ sơ đã có và gắn đặt sân mới vào đó; chỉ tạo hồ sơ không liên kết tài khoản nếu email chưa tồn tại (BR-10). | US-4 |
| KH-13 | Khách thành viên hủy đơn chưa thanh toán của mình tại My Bookings theo BR-05; đơn đã thanh toán xử lý qua nhân viên (BR-23). | US-5 |
| KH-14 | Khách vãng lai hủy đặt sân qua nhân viên (hotline hoặc trực tiếp tại cơ sở). | US-5 |
| KH-15 | Hủy theo yêu cầu khách phải thỏa BR-05; hủy và xử lý hóa đơn theo BR-22/BR-23 trong cùng giao dịch. Hủy lại đơn đã Đã hủy trả kết quả Đã hủy sau kiểm tra quyền, không xử lý tiền hoặc ghi chuyển trạng thái lần nữa. | US-5 |
| KH-16 | Hủy thành công: Đã hủy, ghi lịch sử/nhật ký đúng tác nhân, giải phóng slot; slot chỉ có thể đặt lại nếu sân Hoạt động và lịch khả dụng. Thông báo theo BR-28. | US-5 |
| KH-17 | Khách thành viên đổi ngày/giờ/sân của đơn chưa thanh toán tại My Bookings. Khách vãng lai hoặc đơn đã thanh toán làm việc với nhân viên theo BR-05, BR-23. | US-6 |
| KH-18 | Đổi lịch thỏa BR-05; kiểm tra lịch mới, sân Hoạt động, không trùng và giờ hợp lệ; máy chủ tính giá mới, yêu cầu người thao tác xác nhận tổng tiền trước khi lưu và đồng bộ hóa đơn theo BR-22/BR-23. | US-6 |
| KH-19 | Đổi lịch giữ mã đặt sân và trạng thái hiện tại; giải phóng slot cũ, giữ slot mới, đồng bộ hóa đơn, ghi nhật ký trong một giao dịch. Đổi thất bại giữ nguyên lịch/giá/hóa đơn cũ. Không đổi chủ hồ sơ khách hàng. | US-6 |
| KH-20 | Khách xem đơn của mình, gồm mã, sân, ngày/giờ, tổng tiền, trạng thái sử dụng và trạng thái thanh toán riêng; mới nhất trước (ngày/giờ sử dụng, sau đó thời điểm tạo). Có phân trang và chi tiết; hồ sơ guest đã liên kết sau đăng ký vẫn hiện đầy đủ lịch sử cũ. | US-7 |
| KH-21 | Khách thành viên xem thông tin cá nhân và chỉ tự cập nhật họ tên, số điện thoại. Email chỉ được xem; muốn đổi email phải thông qua nhân viên (BR-18). | US-8 |
| KH-22 | Cung cấp khung trò chuyện với trợ lý AI trên hệ thống, dùng được khi chưa đăng nhập. | US-9 |
| KH-23 | AI tư vấn sân/dịch vụ, hướng dẫn đặt sân, gợi ý sân/giờ theo dữ liệu công khai và giải đáp FAQ theo BR-28; không thực hiện thao tác nghiệp vụ hoặc truy cập dữ liệu cá nhân. | US-9 |

### 4.2 Chức năng nhân viên

| Mã | Yêu cầu | Story |
| --- | --- | --- |
| NV-01 | Nhân viên đăng nhập bằng tài khoản do QTV cấp. Không có chức năng tự đăng ký tài khoản nhân viên. | US-10 |
| NV-02 | Mọi chức năng nhân viên chỉ truy cập được sau khi đăng nhập và đúng quyền theo mục 2.2. | US-10 |
| NV-03 | Danh sách Chờ xác nhận gồm mã, khách, điện thoại, sân, ngày/giờ, tổng tiền, thời điểm tạo; ưu tiên yêu cầu cũ nhất trước, có phân trang và liên kết chi tiết. | US-11 |
| NV-04 | Xem chi tiết một yêu cầu đặt sân. | US-11 |
| NV-05 | Xác nhận Chờ xác nhận trước giờ bắt đầu, kiểm tra sân/lịch/không trùng và tạo hóa đơn theo BR-08. Đã xác nhận là nhân viên đã xác nhận, không phải đã thu tiền. Xác nhận lặp đơn đã Đã xác nhận trả kết quả hiện có, không tạo hóa đơn/lịch sử thêm. | US-12 |
| NV-06 | Từ chối yêu cầu Chờ xác nhận kèm lý do; chuyển Đã hủy, giải phóng chỗ, ghi lịch sử/nhật ký và thông báo khách. Đã xác nhận muốn hủy phải qua BR-05 hoặc BR-24, không dùng thao tác từ chối. | US-12 |
| NV-07 | Chuyển đơn đã tồn tại sang Hoàn thành sau khi nhân viên xác nhận khách đã sử dụng lượt sân và lượt đã kết thúc theo BR-20. Không tạo đơn mới cho lượt đã diễn ra. Việc thanh toán được quản lý độc lập bằng trạng thái hóa đơn. | US-12 |
| NV-08 | Tạo/đổi trạng thái ghi lịch sử/nhật ký trong cùng giao dịch và gửi email khách sau khi lưu theo BR-13, BR-28. Hoàn thành lặp lại trả trạng thái hiện có, không ghi thêm hoặc sửa thanh toán. | US-12 |
| NV-09 | Xem lịch đặt sân theo ngày dạng lưới, bao gồm cả slot trống; lọc theo sân. | US-13 |
| NV-10 | Tạo thay khách tại quầy/hotline: dùng hồ sơ theo email đã xác minh hoặc tạo mới, không thuộc nhân viên. Chỉ tạo trước giờ bắt đầu lượt chơi, khởi tạo Chờ xác nhận và xác nhận qua NV-05; kiểm tra không trùng và các điều kiện BR-20. Ghi nhận sử dụng qua NV-07 chỉ áp dụng cho đơn đã tồn tại. | US-13 |
| NV-11 | Đổi/hủy thay khách theo BR-05, BR-22, BR-23. Hủy sự cố theo BR-24, bắt buộc lý do; không sửa trực tiếp trạng thái để bỏ qua quy tắc. | US-13 |
| NV-12 | Hệ thống ngăn mọi thao tác gây trùng lịch (BR-02). | US-13 |
| NV-13 | Cập nhật trạng thái sân chưa bị xóa: Hoạt động (`ACTIVE`), Bảo trì (`MAINTENANCE`), Đóng (`CLOSED`). Trạng thái Đã xóa (`DELETED`) chỉ được thiết lập qua chức năng xóa sân của QTV. | US-14 |
| NV-14 | Sân không Hoạt động không nhận đặt mới/đổi lịch vào sân hoặc xác nhận yêu cầu thông thường. Chuyển Bảo trì/Đóng bị từ chối khi còn đơn Chờ xác nhận/Đã xác nhận chưa xử lý theo BR-16. Ghi nhật ký. | US-14 |
| NV-15 | Xem danh sách khách hàng; tìm theo họ tên, số điện thoại, email. | US-15 |
| NV-16 | Xem và cập nhật thông tin khách hàng; xem các đặt sân của khách. Đổi email khách hàng phải theo BR-18. | US-15 |
| NV-17 | Tạo tài khoản khách hàng; nếu email đã có hồ sơ chưa liên kết tài khoản thì tái sử dụng hồ sơ đó, nếu chưa có thì tạo hồ sơ mới theo BR-11. | US-15 |
| NV-18 | Hệ thống tự lập hóa đơn theo BR-08, gồm mã hóa đơn, mã đặt sân, tổng tiền, trạng thái thanh toán, nhân viên lập, thời điểm lập, ghi chú; lưu thời điểm và phương thức thanh toán khi hóa đơn được lập ở trạng thái Đã thanh toán. | US-16 |
| NV-19 | Xem danh sách hóa đơn, lọc theo trạng thái (Chưa thanh toán, Đã thanh toán, Đã hủy), tìm theo mã; xem chi tiết hóa đơn. | US-16 |
| NV-20 | Thu đủ tiền: hóa đơn Chưa thanh toán → Đã thanh toán, ghi phương thức/thời điểm thực thu và nhân viên xác nhận; không tự đổi đơn Hoàn thành. Đơn Hoàn thành chưa thu vẫn được thu sau. Không thu đơn Đã hủy/hóa đơn Đã hủy; đơn quá ngày còn Chờ xác nhận/Đã xác nhận phải chốt BR-17 trước. Xác nhận thu lặp trả kết quả hiện có nếu nội dung trùng, khác nội dung bị từ chối; không cập nhật lại thời điểm thu tiền. | US-16 |
| NV-21 | Điều chỉnh hóa đơn cùng việc đổi/hủy đơn theo BR-22, BR-23; xem chuỗi hóa đơn bị hủy và thay thế, lý do, người/thời điểm thao tác. Không để đơn còn hiệu lực thiếu hóa đơn; hoàn tiền đã thu phải có thông tin xác nhận thực tế. | US-16 |
| NV-22 | Tìm kiếm nhanh theo số điện thoại, email hoặc mã đặt sân; trả về khách hàng và các đặt sân liên quan kèm trạng thái, cho phép mở chi tiết ngay. | US-17 |
| NV-23 | Hiển thị số điện thoại hotline trên hệ thống. Nhân viên tiếp nhận và hỗ trợ khách qua điện thoại bên ngoài hệ thống; hệ thống chưa hỗ trợ VoIP. | US-18 |
| NV-24 | Quản lý tài khoản khách hàng: xem, cập nhật thông tin, khóa và mở khóa tài khoản theo BR-12; đổi email theo BR-18. Nhân viên không quản lý tài khoản nội bộ hoặc phân quyền. | US-15 |

### 4.3 Chức năng quản trị viên

Quản trị viên sử dụng được toàn bộ chức năng ở mục 4.2 và các chức năng dưới đây.

| Mã | Yêu cầu | Story |
| --- | --- | --- |
| QT-01 | Đăng nhập khu vực quản trị bằng tài khoản có quyền quản trị. Tài khoản không có quyền bị từ chối truy cập. | US-19 |
| QT-02 | Thêm sân: tên bắt buộc, không trùng tên sân chưa bị xóa; mô tả; trạng thái Hoạt động, Bảo trì hoặc Đóng. | US-20 |
| QT-03 | Sửa và cập nhật thông tin sân chưa bị xóa; tên sân bắt buộc và không trùng tên sân chưa bị xóa khác. | US-20 |
| QT-04 | Xóa mềm sân: chuyển trạng thái sang Đã xóa (`DELETED`) và xóa giá trị tên sân theo BR-19; giữ nguyên bản ghi và các liên kết giao dịch. Không được xóa sân khi còn đặt sân Chờ xác nhận hoặc Đã xác nhận chưa được xử lý. | US-20 |
| QT-05 | Tạo sân yêu cầu nhập giờ mở/đóng và đơn giá mặc định; hệ thống tạo 7 lịch và 7 mốc giá theo BR-25 trong cùng giao dịch. Mỗi ngày đúng một lịch; sửa giờ/khả dụng theo BR-01, BR-16. | US-20 |
| QT-06 | Cấu hình mốc giá tăng dần, không trùng, đơn giá > 0 theo BR-03; lưu thay toàn bộ bảng giá ngày đó. Đổi giờ mở/đóng xem trước cách điều chỉnh giá theo BR-25 rồi xác nhận; không làm đổi giá đã lưu trên đơn cũ. | US-20 |
| QT-07 | Từ chối thay đổi lịch mở cửa khi vi phạm BR-16; ngày ngừng mở cửa được đánh dấu không khả dụng, không xóa lịch (BR-01). Thay đổi bảng giá không làm đổi tổng tiền của các đặt sân đã tạo (BR-04). | US-20 |
| QT-08 | Tạo tài khoản cho khách hàng, nhân viên, quản trị viên. | US-21 |
| QT-09 | Cập nhật thông tin tài khoản trong phạm vi loại hồ sơ hiện có theo BR-27. Đổi email khách theo BR-18; đổi email nội bộ phải đồng bộ email hồ sơ Nhân viên/email đăng nhập, xác minh email mới, không trùng, vô hiệu mã xác minh cũ và ghi nhật ký. Không sửa trực tiếp bộ đếm, mật khẩu băm hoặc trạng thái để bỏ qua chức năng khóa/mở khóa. | US-21 |
| QT-10 | Khóa/mở khóa tài khoản khách hàng, nhân viên, QTV bằng DISABLED/ACTIVE theo BR-12. QTV được hard-delete tài khoản theo BR-15, kể cả tài khoản khách; giữ hồ sơ và lịch sử, ghi nhật ký và bảo vệ QTV ACTIVE cuối cùng. | US-21 |
| QT-11 | Thay quyền Nhân viên ↔ QTV trên tài khoản nội bộ theo BR-27; không chuyển loại hồ sơ Khách hàng ↔ Nhân viên. Bảo vệ QTV ACTIVE cuối cùng; quyền trên máy chủ có hiệu lực theo dữ liệu hiện tại. | US-21 |
| QT-12 | Thêm nhân viên gồm hồ sơ (họ tên, số điện thoại, email, có quyền quản trị hay không) và tài khoản đăng nhập. | US-22 |
| QT-13 | Sửa họ tên, số điện thoại và email hồ sơ nhân viên; email đăng nhập liên quan cập nhật theo QT-09, thay quyền theo QT-11, không sửa độc lập làm lệch tài khoản/hồ sơ. | US-22 |
| QT-14 | Xóa tài khoản nhân viên là hard-delete tài khoản đăng nhập theo BR-15; giữ hồ sơ và mã nhân viên cho lịch sử/hóa đơn/nhật ký, đặt accountId NULL, không xóa vật lý hồ sơ hoặc giao dịch khách. Tuân thủ BR-27. | US-22 |
| QT-15 | Xem toàn bộ đặt sân của hệ thống; lọc theo ngày, sân, trạng thái, khách hàng; xem chi tiết kèm lịch sử trạng thái. | US-23 |
| QT-16 | Hỗ trợ xử lý phát sinh bằng các thao tác xác nhận, hủy, đổi lịch, chuyển trạng thái như nhân viên. | US-23 |
| QT-17 | Thống kê doanh thu theo khoảng thời gian do QTV chọn: tổng doanh thu và phân theo ngày hoặc tháng, hiển thị dạng bảng và biểu đồ. | US-24 |
| QT-18 | Doanh thu ghi nhận là tổng tiền hóa đơn hợp lệ Đã thanh toán có thời điểm thu trong khoảng lọc; không cộng hóa đơn Đã hủy hoặc bản thay thế chưa thu. Hóa đơn đã hoàn/hủy bị loại kể cả thời điểm thu thuộc kỳ cũ, nên báo cáo kỳ cũ có thể đổi sau hoàn tiền; chỉ tiêu này không phải báo cáo dòng tiền thu/hoàn. | US-24 |
| QT-19 | Thống kê lượt đặt và giờ đã đặt (mọi trạng thái trừ Đã hủy), tách lượt/giờ Hoàn thành. Lọc theo ngày sử dụng; giữ giao dịch sân Đã xóa với nhãn lịch sử. Chỉ tiêu Hoàn thành gồm cả chốt tự động BR-17, không xác nhận khách có mặt. | US-25 |
| QT-20 | Xem nhật ký hoạt động gồm thời điểm, tác nhân (hệ thống, nhân viên, khách hàng), thao tác, đối tượng, giá trị trước và sau thay đổi. | US-26 |
| QT-21 | Lọc nhật ký theo thời gian, tác nhân, đối tượng, thao tác. Nhật ký chỉ đọc, không cho sửa hoặc xóa. | US-26 |

## 5. Yêu cầu dữ liệu

| Dữ liệu | Thông tin lưu trữ |
| --- | --- |
| Tài khoản | Mã tài khoản, email đăng nhập chuẩn hóa (duy nhất), mật khẩu dạng băm, loại tài khoản (Khách hàng, Nhân viên), trạng thái Hoạt động (`ACTIVE`) hoặc Khóa/Vô hiệu hóa (`DISABLED`), số lần đăng nhập sai liên tiếp. Mở khóa thủ công theo BR-12; hard-delete theo BR-15. |
| Nhân viên | Họ tên, số điện thoại, email, quyền quản trị, tài khoản liên kết. |
| Khách hàng | Họ tên, số điện thoại, email (duy nhất), tài khoản liên kết (có thể trống với khách vãng lai). |
| Mã xác minh | Email chuẩn hóa, mục đích, mã băm, số lần sai, đã dùng, thời điểm hết hạn; khóa `(email, type)`. Giới hạn gửi và ngữ cảnh xác minh đặt nhanh do dịch vụ xử lý theo BR-26. |
| Sân | Mã sân, tên (duy nhất, bắt buộc khi chưa bị xóa, NULL khi Đã xóa), mô tả, trạng thái Hoạt động (`ACTIVE`), Bảo trì (`MAINTENANCE`), Đóng (`CLOSED`), Đã xóa (`DELETED`). |
| Lịch mở cửa | Sân, ngày trong tuần, giờ mở, giờ đóng, khả dụng; duy nhất theo cặp sân và ngày trong tuần. Mỗi sân có đúng một lịch cho mỗi ngày trong tuần. |
| Bảng giá | Sân, ngày trong tuần, mốc giờ bắt đầu áp dụng giá, đơn giá theo giờ > 0; duy nhất theo sân, ngày trong tuần và mốc giờ. Mốc giờ tăng dần; không lưu giờ kết thúc riêng (BR-03). |
| Đặt sân | Mã đặt sân, khách hàng, sân, ngày sử dụng, giờ bắt đầu, giờ kết thúc, trạng thái Chờ xác nhận (`PENDING`), Đã xác nhận (`CONFIRMED`), Hoàn thành (`COMPLETED`), Đã hủy (`CANCELLED`), tổng tiền VND đã làm tròn, các mốc/đơn giá đã áp dụng. Ý nghĩa trạng thái theo BR-06; tự xử lý qua ngày theo BR-17. |
| Lịch sử trạng thái đặt sân | Đặt sân, trạng thái cũ (trống khi tạo), trạng thái mới, loại tác nhân, mã khách hoặc mã nhân viên khi có, thời điểm, lý do; chỉ một loại người thực hiện trên mỗi bản ghi. |
| Hóa đơn | Mã hóa đơn, đặt sân, tổng tiền, trạng thái Chưa thanh toán (`UNPAID`), Đã thanh toán (`PAID`), Đã hủy (`CANCELLED`), nhân viên lập/thu/hủy/hoàn, thời điểm lập/thu/hủy/hoàn, phương thức thu/hoàn, số tiền hoàn, lý do, mã hóa đơn bị thay thế, ghi chú; giữ dấu vết thu tiền sau khi hủy. |
| Nhật ký hệ thống | Tác nhân (hệ thống, nhân viên, khách hàng), mã người thực hiện, thao tác, đối tượng, mã đối tượng, giá trị trước/sau đã loại bí mật, lý do, thời điểm. |
| Chống gửi lặp | Mã yêu cầu, phạm vi người thao tác, nội dung/định danh thao tác, kết quả đã lưu và mã bản ghi liên quan theo BR-21. |
| Thông báo | Đối tượng nhận, mã đơn/sự kiện, trạng thái gửi, số lần thử và lỗi gần nhất; không chứa mật khẩu hoặc mã phiên. |

## 6. Yêu cầu phi chức năng

| Mã | Nhóm | Yêu cầu |
| --- | --- | --- |
| NFR-01 | Bảo mật | Mật khẩu và mã xác minh chỉ lưu dạng băm, không lưu bản rõ. |
| NFR-02 | Bảo mật | Quyền truy cập được kiểm tra tại máy chủ theo ma trận phân quyền mục 2.2, không chỉ dựa vào việc ẩn chức năng trên giao diện. |
| NFR-03 | Bảo mật | Sau 5 lần đăng nhập sai liên tiếp, tài khoản bị khóa ngay ở lần sai thứ 5; không tự mở khóa và phải đợi nhân viên hoặc QTV có quyền mở khóa theo BR-12. |
| NFR-04 | Toàn vẹn dữ liệu | Việc chống trùng lịch (BR-02) được bảo đảm ở tầng cơ sở dữ liệu, kể cả khi nhiều người đặt cùng một sân tại cùng thời điểm. |
| NFR-05 | Toàn vẹn dữ liệu | Không xóa vật lý dữ liệu giao dịch và sân; xóa mềm sân phải đồng thời giải phóng tên theo BR-15, BR-19. |
| NFR-06 | Truy vết | Mọi thao tác theo BR-14 được ghi nhật ký và không thể sửa, xóa qua hệ thống. |
| NFR-07 | Thời gian | Các mốc thời gian của hệ thống (tạo, cập nhật, nhật ký, hết hạn mã xác minh) được lưu theo giờ UTC. Việc xác định ngày sử dụng và tự xử lý qua ngày mới dùng giờ Việt Nam (UTC+07:00) theo BR-17. |
| NFR-08 | Toàn vẹn dữ liệu | Giao dịch nguyên tử, duy nhất email/tài khoản/hóa đơn, kiểm soát xung đột và gửi lặp theo BR-21 phải được bảo đảm phía máy chủ; không dựa vào nút bị vô hiệu trên trình duyệt. |
| NFR-09 | Khả năng phục hồi | Chốt ngày BR-17 chạy bù sau gián đoạn, không xử lý lại trạng thái cuối; lỗi gửi thông báo không làm mất hoặc tạo lặp giao dịch đã lưu. |
| NFR-10 | Bảo mật | Kiểm tra chủ sở hữu trên mọi API đọc/sửa dữ liệu khách; kiểm tra tài khoản còn tồn tại, ACTIVE và quyền hiện tại theo BR-12, BR-27; không ghi bí mật vào log, không dùng AI để vượt quyền. |
| NFR-11 | Khả năng kiểm thử | Thời điểm nghiệp vụ lấy từ đồng hồ máy chủ thống nhất; kiểm thử được các biên đúng 1 giờ, giờ kết thúc, 00:00 Việt Nam và các thao tác cạnh tranh. |

## 7. Thay đổi tài liệu

| Phiên bản | Thay đổi |
| --- | --- |
| 1.2 | Làm rõ vòng đời đặt sân, hóa đơn, xác minh email và xử lý đồng thời. |
| 1.3 | Bỏ tạo đơn cho lượt đã diễn ra và lịch sử giá phục vụ luồng này. |
| 1.4 | Gộp khóa/vô hiệu hóa thành DISABLED; hard-delete tài khoản; bỏ quản lý phiên và cột version. |
| 1.5 | Giữ VerificationCode theo `(email, type)`; bỏ lịch sử gửi mã, lịch mở kế hoạch và tỷ lệ lấp đầy; rút gọn tài liệu. |
