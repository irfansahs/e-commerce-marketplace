# Güvenlik

## Asla repoya commit etmeyin

- `.env` (gerçek değerler)
- SSH private key (`*.pem`, `*.key`)
- AWS access keys, payment API secrets

`.env.example` yalnızca placeholder içerir.

## AWS / EC2 deploy (Faz C)

- GitHub Actions **Secrets**: `EC2_HOST`, `EC2_USER`, `EC2_SSH_KEY`
- Public README veya issue’larda gerçek IP veya key path yazmayın
- EC2 security group: yalnızca gerekli portlar (22, 80/443, vb.)
- PEM rotasyonu: key sızdıysa AWS’de yeni key pair oluşturup eskisini devre dışı bırakın

## Uygulama

- JWT secret’lar environment variable
- Gateway’de JWT doğrulama; servisler trust boundary net
- RabbitMQ ve SQL Server prod’da güçlü parola + network izolasyonu
