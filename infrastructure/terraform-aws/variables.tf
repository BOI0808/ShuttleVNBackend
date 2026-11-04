variable "project_name" {
  type        = string
  description = "Name of the project"
}

variable "ami_id" {
  type        = string
  description = "Pinned Ubuntu amd64 AMI ID in Singapore."
}

variable "ssh_public_key_path" {
  type        = string
  description = "Path to the SSH public key file."
}

variable "ssh_allowed_cidr" {
  type        = string
  description = "Administrator public IPv4 address with /32."
}