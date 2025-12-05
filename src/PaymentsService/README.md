# PaymentsService

Purpose: Handles payment creation and status. Stores payments in Postgres and exposes both REST and gRPC.

Local ports:
- REST: http://localhost:5001/api/payments
- gRPC: port 5002 (see docker-compose)

Quick start:
1. Build (repo root): `scripts/run-local.ps1`
2. Swagger UI: http://localhost:5001/swagger
3. Protos: `src/BuildingBlocks/protos/payments.proto`

What to show:
- Domain model (`Domain/Payment.cs`) enforcing invariants.
- gRPC service implementation `Grpc/PaymentsGrpcService.cs`.
- RabbitMQ publishing on payment creation.
