# ParkRD React Frontend - Final Updated

Frontend actualizado para consumir el backend real de ParkRD.

## Incluye

- Billetera conectada al backend.
- Recargas mediante `POST /api/wallets/add-amount`.
- Movimientos reales de billetera.
- Reportes conectados a `GET /api/reports/...`.
- Cancelación de reservas mediante `PUT /api/reservations/cancel/{id}`.
- Eliminación administrativa conserva el endpoint existente `DELETE /api/reservations/{id}`.
- Proxy de Vite apuntando a `https://localhost:7183`.

## Ejecutar

Primero ejecutar el backend `ParkRD.API` y confirmar que Swagger abre en:

```text
https://localhost:7183/swagger/index.html
```

Luego ejecutar el frontend:

```powershell
npm install
npm run dev
```

Abrir:

```text
https://localhost:3000
```
