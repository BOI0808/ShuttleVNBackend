resource "aws_security_group" "public" {
  name        = "${var.project_name}-public-sg"
  description = "Frontend and reverse proxy"
  vpc_id      = var.vpc_id

  tags = {
    Name = "${var.project_name}-public-sg"
  }
}

resource "aws_security_group" "private" {
  name        = "${var.project_name}-private-sg"
  description = "Backend and monitoring"
  vpc_id      = var.vpc_id

  tags = {
    Name = "${var.project_name}-private-sg"
  }
}

resource "aws_vpc_security_group_ingress_rule" "public_ingress" {
  for_each = {
    ssh = {
      port = 22
      cidr = var.ssh_allowed_cidr
    }
    http = {
      port = 80
      cidr = "0.0.0.0/0"
    }
    https = {
      port = 443
      cidr = "0.0.0.0/0"
    }
  }

  security_group_id = aws_security_group.public.id
  description       = "Allow ${each.key}"
  ip_protocol       = "tcp"
  from_port         = each.value.port
  to_port           = each.value.port
  cidr_ipv4         = each.value.cidr
}

resource "aws_vpc_security_group_ingress_rule" "private_ingress" {
  for_each = {
    ssh     = 22
    backend = 8080
  }

  security_group_id            = aws_security_group.private.id
  referenced_security_group_id = aws_security_group.public.id
  description                  = "Allow ${each.key} from public SG"
  ip_protocol                  = "tcp"
  from_port                    = each.value
  to_port                      = each.value
}

resource "aws_vpc_security_group_ingress_rule" "public_metrics" {
  for_each = {
    node_exporter = 9100
    cadvisor      = 8081
  }

  security_group_id            = aws_security_group.public.id
  referenced_security_group_id = aws_security_group.private.id
  description                  = "Allow ${each.key} from private SG"
  ip_protocol                  = "tcp"
  from_port                    = each.value
  to_port                      = each.value
}

resource "aws_vpc_security_group_egress_rule" "outbound" {
  for_each = {
    public  = aws_security_group.public.id
    private = aws_security_group.private.id
  }

  security_group_id = each.value
  description       = "Allow outbound traffic"
  ip_protocol       = "-1"
  cidr_ipv4         = "0.0.0.0/0"
}