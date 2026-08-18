
import React, { useEffect, useMemo, useState } from "react";

const apiUrl = "/api";

const emptyUser = { firstName: "", lastName: "", nationalId: "", email: "", phoneNumber: "" };
const emptyVehicle = { plate: "", brand: "", model: "", color: "" };
const emptyParking = { code: "", hourlyRate: "", dailyRate: "" };

function today() {
  return new Date().toISOString().split("T")[0];
}

function dateOnly(value) {
  return String(value || "").split("T")[0].split(" ")[0];
}

function minutes(value) {
  const match = String(value || "").match(/(\d{1,2}):(\d{2})/);
  if (!match) return 0;
  return Number(match[1]) * 60 + Number(match[2]);
}

function ampm(value) {
  const match = String(value || "").match(/(\d{1,2}):(\d{2})/);
  if (!match) return String(value || "");
  let h = Number(match[1]);
  const m = match[2];
  const p = h >= 12 ? "PM" : "AM";
  h = h % 12 || 12;
  return `${h}:${m} ${p}`;
}

function range(start, end) {
  return `${ampm(start)} - ${ampm(end)}`;
}

function endTime(start, duration) {
  const [h, m] = String(start || "08:00").split(":").map(Number);
  const d = new Date(2026, 0, 1, h || 0, m || 0);
  d.setHours(d.getHours() + Number(duration || 1));
  return `${String(d.getHours()).padStart(2, "0")}:${String(d.getMinutes()).padStart(2, "0")}`;
}

function money(value) {
  return `RD$${Number(value || 0).toLocaleString("es-DO")}`;
}

function isCancelled(status) {
  return String(status || "").toLowerCase().includes("cancel");
}

function transactionTypeLabel(value) {
  const type = String(value || "").toLowerCase();
  if (type.includes("recharge")) return "Recarga";
  if (type.includes("payment")) return "Pago";
  if (type.includes("refund")) return "Devolución";
  return value || "Movimiento";
}

function walletAmountValue(item) {
  const amount = Number(item?.amount || 0);
  const type = String(item?.transactionType || item?.type || "").toLowerCase();
  if (type.includes("payment")) return -Math.abs(amount);
  return Math.abs(amount);
}

function dateTimeLabel(value) {
  if (!value) return "";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return dateOnly(value);
  return date.toLocaleString("es-DO", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit"
  });
}

function reportEndpoint(type) {
  return {
    users: "users",
    parkings: "parkings",
    vehicles: "vehicles",
    reservations: "reservations",
    wallets: "wallets",
    "wallet-transactions": "wallet-transactions"
  }[type] || "users";
}

async function api(path, options = {}) {
  const res = await fetch(`${apiUrl}${path}`, {
    headers: { "Content-Type": "application/json", ...(options.headers || {}) },
    ...options
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || "Error al conectar con la API");
  }

  if (res.status === 204) return null;
  const contentType = res.headers.get("content-type") || "";
  return contentType.includes("application/json") ? res.json() : null;
}

function Toast({ message }) {
  return message ? <div className="toast">{message}</div> : null;
}

function Home({ onEnter }) {
  return (
    <main className="home">
      <section className="login-card">
        <div className="login-top">
          <img src="/assets/parkrd-logo.png" alt="ParkRD" />
          <h1>Acceso al sistema</h1>
          <p>Selecciona cómo deseas entrar al sistema ParkRD.</p>
        </div>

        <div className="role-grid">
          <article>
            <span>Vista Usuario</span>
            <h2>Usuario</h2>
            <p>Reserva espacios y consulta tus vehículos.</p>
            <button className="btn btn-red" onClick={() => onEnter("user")}>Entrar como usuario</button>
          </article>
          <article>
            <span>Vista Admin</span>
            <h2>Administrador</h2>
            <p>Gestiona usuarios, parqueos, vehículos y reservas.</p>
            <button className="btn btn-blue" onClick={() => onEnter("admin")}>Entrar como admin</button>
          </article>
        </div>
      </section>
    </main>
  );
}

function Icon({ name }) {
  const common = {
    width: 22,
    height: 22,
    viewBox: "0 0 24 24",
    fill: "none",
    stroke: "currentColor",
    strokeWidth: 1.9,
    strokeLinecap: "round",
    strokeLinejoin: "round",
    "aria-hidden": "true"
  };

  const icons = {
    home: (
      <svg {...common}>
        <path d="M3 10.5 12 3l9 7.5" />
        <path d="M5 9.5V21h14V9.5" />
        <path d="M9 21v-6h6v6" />
      </svg>
    ),
    user: (
      <svg {...common}>
        <circle cx="12" cy="8" r="4" />
        <path d="M4.5 21a7.5 7.5 0 0 1 15 0" />
      </svg>
    ),
    parking: (
      <svg {...common}>
        <rect x="4" y="4" width="16" height="16" rx="3" />
        <path d="M9 17V7h4a3 3 0 0 1 0 6H9" />
      </svg>
    ),
    car: (
      <svg {...common}>
        <path d="M6 17h12" />
        <path d="M5 17l1.5-5.5A3 3 0 0 1 9.4 9h5.2a3 3 0 0 1 2.9 2.5L19 17" />
        <circle cx="7.5" cy="17.5" r="1.5" />
        <circle cx="16.5" cy="17.5" r="1.5" />
        <path d="M8 13h8" />
      </svg>
    ),
    calendar: (
      <svg {...common}>
        <rect x="4" y="5" width="16" height="16" rx="2.5" />
        <path d="M8 3v4M16 3v4M4 10h16" />
      </svg>
    ),
    wallet: (
      <svg {...common}>
        <path d="M4 7.5A2.5 2.5 0 0 1 6.5 5H19a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H6.5A2.5 2.5 0 0 1 4 16.5v-9Z" />
        <path d="M4 8h15" />
        <path d="M15 13h5v4h-5a2 2 0 0 1 0-4Z" />
        <path d="M17 15h.01" />
      </svg>
    ),
    chart: (
      <svg {...common}>
        <path d="M4 19h16" />
        <path d="M7 16V9" />
        <path d="M12 16V5" />
        <path d="M17 16v-4" />
      </svg>
    ),
    swap: (
      <svg {...common}>
        <path d="M7 7h13l-4-4" />
        <path d="M17 17H4l4 4" />
      </svg>
    )
  };

  return icons[name] || icons.home;
}

function Sidebar({ mode, view, onView, onHome }) {
  const adminItems = [
    ["admin", "Dashboard", "home"],
    ["admin-users", "Usuarios", "user"],
    ["admin-parkings", "Parqueos", "parking"],
    ["admin-vehicles", "Vehículos", "car"],
    ["admin-reservations", "Reservas", "calendar"],
    ["admin-wallet", "Billetera", "wallet"],
    ["admin-reports", "Reportes", "chart"]
  ];

  return (
    <aside className="sidebar clean-sidebar">
      <div className="brand logo-only">
        <div className="brand-mark">
          <img src="/assets/parkrd-logo.png" alt="ParkRD" />
        </div>
      </div>

      <button className="back-btn icon-link" onClick={onHome}>
        <Icon name="swap" />
        <span>Cambiar vista</span>
      </button>

      <div className="divider" />

      <nav className="nav-list">
        {mode === "user" && (
          <>
            <button className={view === "user" ? "active icon-link" : "icon-link"} onClick={() => onView("user")}>
              <Icon name="user" />
              <span>Vista Usuario</span>
            </button>
            <button className={view === "user-wallet" ? "active icon-link" : "icon-link"} onClick={() => onView("user-wallet")}>
              <Icon name="wallet" />
              <span>Billetera</span>
            </button>
          </>
        )}

        {mode === "admin" && adminItems.map(([id, text, icon]) => (
          <button key={id} className={view === id ? "active icon-link" : "icon-link"} onClick={() => onView(id)}>
            <Icon name={icon} />
            <span>{text}</span>
          </button>
        ))}
      </nav>
    </aside>
  );
}


function Topbar({ mode, view }) {
  const titles = {
    user: "Vista Usuario",
    "user-wallet": "Billetera",
    admin: "Panel administrativo",
    "admin-users": "Usuarios",
    "admin-parkings": "Parqueos",
    "admin-vehicles": "Vehículos",
    "admin-reservations": "Reservas",
    "admin-wallet": "Billetera",
    "admin-reports": "Reportes"
  };

  const role = mode === "admin" ? "Administrador" : "Usuario";
  const initials = mode === "admin" ? "AD" : "US";

  return (
    <header className="topbar">
      <div className="topbar-left">
        <button className="menu-btn" type="button" aria-label="Menú">
          <span></span><span></span><span></span>
        </button>
        <strong>{titles[view] || "ParkRD"}</strong>
      </div>

      <div className="topbar-right">
        <button className="bell-btn" type="button" aria-label="Notificaciones">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M18 8a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9" />
            <path d="M13.7 21a2 2 0 0 1-3.4 0" />
          </svg>
        </button>
        <div className="admin-pill">
          <span>{initials}</span>
          <p>{role}</p>
          <small>⌄</small>
        </div>
      </div>
    </header>
  );
}

function Header({ title, subtitle, right }) {
  return (
    <header className="header">
      <div>
        <h1>{title}</h1>
        <p>{subtitle}</p>
      </div>
      {right}
    </header>
  );
}

function StatCard({ label, value }) {
  return (
    <article className="stat-card">
      <span>{label}</span>
      <strong>{value}</strong>
    </article>
  );
}

function DataTable({ columns, children }) {
  return (
    <div className="table-wrap">
      <table>
        <thead>
          <tr>{columns.map(c => <th key={c}>{c}</th>)}</tr>
        </thead>
        <tbody>
          {children || <tr><td colSpan={columns.length}>Sin datos.</td></tr>}
        </tbody>
      </table>
    </div>
  );
}


function ConfirmDialog({ data, onCancel, onConfirm }) {
  if (!data) return null;

  return (
    <div className="confirm-backdrop">
      <section className="confirm-card">
        <div className="confirm-icon">!</div>
        <div className="confirm-content">
          <h2>{data.title || "Confirmar acción"}</h2>
          <p>{data.message || "¿Deseas continuar?"}</p>
        </div>
        <div className="confirm-actions">
          <button className="confirm-secondary" type="button" onClick={onCancel}>Cancelar</button>
          <button className="confirm-primary" type="button" onClick={onConfirm}>Sí, continuar</button>
        </div>
      </section>
    </div>
  );
}

function ReservationModal({ open, parking, vehicles, currentUserId, onClose, onCreate }) {
  const [type, setType] = useState("Hour");
  const [vehicleId, setVehicleId] = useState("");
  const [date, setDate] = useState(today());
  const [start, setStart] = useState("08:00");
  const [duration, setDuration] = useState("2");
  const [startDate, setStartDate] = useState(today());
  const [endDate, setEndDate] = useState(today());

  useEffect(() => {
    if (open) {
      setType("Hour");
      setVehicleId("");
      setDate(today());
      setStart("08:00");
      setDuration("2");
      setStartDate(today());
      setEndDate(today());
    }
  }, [open]);

  if (!open || !parking) return null;

  const userVehicles = vehicles.filter(v => Number(v.userId) === Number(currentUserId));

  const submit = () => {
    if (!vehicleId) return onCreate(null, "Selecciona un vehículo.");

    if (type === "Hour") {
      const end = endTime(start, duration);
      onCreate({
        userId: Number(currentUserId),
        parkingId: Number(parking.id),
        vehicleId: Number(vehicleId),
        reservationDate: date,
        startTime: `${start}:00`,
        endTime: `${end}:00`,
        reservationType: "Hour",
        totalAmount: Number(parking.hourlyRate || 0) * Number(duration || 1),
        status: "Reserved"
      });
      return;
    }

    const days = Math.max(1, Math.ceil((new Date(endDate) - new Date(startDate)) / 86400000) + 1);
    onCreate({
      userId: Number(currentUserId),
      parkingId: Number(parking.id),
      vehicleId: Number(vehicleId),
      reservationDate: startDate,
      startTime: "00:00:00",
      endTime: "23:59:00",
      reservationType: "Day",
      totalAmount: Number(parking.dailyRate || 0) * days,
      status: "Reserved"
    });
  };

  return (
    <div className="modal-backdrop">
      <section className="modal">
        <button className="modal-close" onClick={onClose}>×</button>
        <h2>Reservar {parking.code}</h2>
        <p>Completa los datos para confirmar la reserva.</p>

        <label>Tipo de reserva</label>
        <div className="toggle">
          <button className={type === "Hour" ? "active" : ""} onClick={() => setType("Hour")}>Por hora</button>
          <button className={type === "Day" ? "active" : ""} onClick={() => setType("Day")}>Por día</button>
        </div>

        <label>Vehículo</label>
        <select value={vehicleId} onChange={e => setVehicleId(e.target.value)}>
          <option value="">Selecciona un vehículo</option>
          {userVehicles.map(v => <option key={v.id} value={v.id}>{v.plate} - {v.brand} {v.model}</option>)}
        </select>

        {type === "Hour" ? (
          <>
            <label>Fecha</label>
            <input type="date" value={date} onChange={e => setDate(e.target.value)} />

            <label>Hora inicio</label>
            <select value={start} onChange={e => setStart(e.target.value)}>
              {Array.from({ length: 15 }, (_, i) => 7 + i).map(h => (
                <option key={h} value={`${String(h).padStart(2, "0")}:00`}>
                  {ampm(`${String(h).padStart(2, "0")}:00`)}
                </option>
              ))}
            </select>

            <label>Duración</label>
            <select value={duration} onChange={e => setDuration(e.target.value)}>
              {[1, 2, 3, 4, 5, 6, 8].map(h => <option key={h} value={h}>{h} hora{h > 1 ? "s" : ""}</option>)}
            </select>
          </>
        ) : (
          <>
            <label>Fecha entrada</label>
            <input type="date" value={startDate} onChange={e => setStartDate(e.target.value)} />
            <label>Fecha salida</label>
            <input type="date" value={endDate} onChange={e => setEndDate(e.target.value)} />
          </>
        )}

        <button className="btn btn-red full" onClick={submit}>Confirmar reserva</button>
      </section>
    </div>
  );
}

export default function App() {
  const [mode, setMode] = useState("");
  const [view, setView] = useState("home");
  const [toast, setToast] = useState("");
  const [confirmData, setConfirmData] = useState(null);

  const [users, setUsers] = useState([]);
  const [parkings, setParkings] = useState([]);
  const [vehicles, setVehicles] = useState([]);
  const [reservations, setReservations] = useState([]);
  const [wallets, setWallets] = useState([]);

  const [currentUserId, setCurrentUserId] = useState("");
  const [filterDate, setFilterDate] = useState(today());
  const [filterStart, setFilterStart] = useState("08:00");
  const [filterDuration, setFilterDuration] = useState("2");
  const [filterMode, setFilterMode] = useState("available");

  const [vehicleForm, setVehicleForm] = useState(emptyVehicle);
  const [userForm, setUserForm] = useState(emptyUser);
  const [parkingForm, setParkingForm] = useState(emptyParking);
  const [adminVehicleForm, setAdminVehicleForm] = useState({ userId: "", plate: "", brand: "", model: "", color: "" });
  const [walletForm, setWalletForm] = useState({ userId: "", amount: "", description: "Recarga de billetera" });
  const [walletTransactions, setWalletTransactions] = useState([]);
  const [reportType, setReportType] = useState("users");
  const [reportRows, setReportRows] = useState([]);
  const [reportLoading, setReportLoading] = useState(false);
  const [modalParking, setModalParking] = useState(null);

  const showToast = (message) => {
    setToast(String(message || ""));
    setTimeout(() => setToast(""), 3000);
  };

  const askConfirm = ({ title = "Confirmar acción", message = "¿Deseas continuar?" }) => {
    return new Promise(resolve => {
      setConfirmData({
        title,
        message,
        onResult: resolve
      });
    });
  };

  const closeConfirm = (result) => {
    if (confirmData?.onResult) confirmData.onResult(result);
    setConfirmData(null);
  };

  const loadAll = async () => {
    try {
      const [u, p, v, r, w, wt] = await Promise.all([
        api("/users").catch(() => []),
        api("/parkings").catch(() => []),
        api("/vehicles").catch(() => []),
        api("/reservations").catch(() => []),
        api("/wallets").catch(() => []),
        api("/wallets/transactions").catch(() => [])
      ]);

      setUsers(Array.isArray(u) ? u : []);
      setParkings(Array.isArray(p) ? p : []);
      setVehicles(Array.isArray(v) ? v : []);
      setReservations(Array.isArray(r) ? r : []);
      setWallets(Array.isArray(w) ? w : []);
      setWalletTransactions(Array.isArray(wt) ? wt : []);

      if (!currentUserId && Array.isArray(u) && u.length) setCurrentUserId(String(u[0].id));
    } catch (error) {
      showToast(error.message);
    }
  };

  useEffect(() => { loadAll(); }, []);

  useEffect(() => {
    if (!currentUserId) return;

    const loadCurrentWallet = async () => {
      try {
        const [wallet, transactions] = await Promise.all([
          api(`/wallets/user/${currentUserId}`).catch(() => null),
          api(`/wallets/user/${currentUserId}/transactions`).catch(() => [])
        ]);

        if (wallet) {
          setWallets(previous => {
            const others = previous.filter(item => Number(item.userId) !== Number(currentUserId));
            return [...others, wallet];
          });
        }

        if (Array.isArray(transactions)) {
          setWalletTransactions(previous => {
            const others = previous.filter(item => Number(item.userId) !== Number(currentUserId));
            return [...transactions, ...others].sort((a, b) => new Date(b.createdAt || 0) - new Date(a.createdAt || 0));
          });
        }
      } catch {
        // La vista sigue funcionando aunque no exista una billetera para el usuario seleccionado.
      }
    };

    loadCurrentWallet();
  }, [currentUserId]);

  useEffect(() => {
    if (mode !== "admin" || view !== "admin-reports") return;

    const loadReport = async () => {
      setReportLoading(true);
      try {
        const data = await api(`/reports/${reportEndpoint(reportType)}`);
        setReportRows(Array.isArray(data) ? data : []);
      } catch (error) {
        setReportRows([]);
        showToast(error.message);
      } finally {
        setReportLoading(false);
      }
    };

    loadReport();
  }, [mode, view, reportType]);

  const goHome = () => {
    setMode("");
    setView("home");
  };

  const enterMode = (nextMode) => {
    setMode(nextMode);
    setView(nextMode === "admin" ? "admin" : "user");
    loadAll();
  };

  const hasConflict = (parkingId, date, start, end) => {
    const d = dateOnly(date);
    const s = minutes(start);
    const e = minutes(end);

    return reservations.some(r => {
      if (Number(r.parkingId) !== Number(parkingId)) return false;
      if (isCancelled(r.status)) return false;
      if (dateOnly(r.reservationDate || r.date) !== d) return false;
      return s < minutes(r.endTime) && e > minutes(r.startTime);
    });
  };

  const filteredParkings = useMemo(() => {
    const start = `${filterStart}:00`;
    const end = `${endTime(filterStart, filterDuration)}:00`;

    return parkings
      .map(parking => ({
        parking,
        reserved: hasConflict(parking.id, filterDate, start, end),
        countDay: reservations.filter(r =>
          Number(r.parkingId) === Number(parking.id) &&
          dateOnly(r.reservationDate || r.date) === dateOnly(filterDate) &&
          !isCancelled(r.status)
        ).length
      }))
      .filter(item => {
        if (filterMode === "available") return !item.reserved;
        if (filterMode === "reserved") return item.reserved;
        return true;
      });
  }, [parkings, reservations, filterDate, filterStart, filterDuration, filterMode]);

  const userVehicles = vehicles.filter(v => Number(v.userId) === Number(currentUserId));
  const userReservations = reservations.filter(r => Number(r.userId) === Number(currentUserId));
  const currentWallet = wallets.find(w => Number(w.userId) === Number(currentUserId));
  const currentUserWalletTransactions = walletTransactions.filter(t => Number(t.userId) === Number(currentUserId));
  const currentUserBalance = currentWallet
    ? Number(currentWallet.balance || 0)
    : currentUserWalletTransactions.reduce((sum, item) => sum + walletAmountValue(item), 0);
  const walletBalance = wallets.reduce((total, item) => total + Number(item.balance || 0), 0);

  const buildReport = (type, rows) => {
    const source = Array.isArray(rows) ? rows : [];

    const definitions = {
      users: {
        name: "Usuarios",
        columns: ["ID", "Nombre", "Cédula", "Email", "Teléfono"],
        rows: source.map(u => ({
          ID: u.id,
          Nombre: `${u.firstName || ""} ${u.lastName || ""}`.trim(),
          Cédula: u.nationalId || "",
          Email: u.email || "",
          Teléfono: u.phoneNumber || ""
        }))
      },
      parkings: {
        name: "Parqueos",
        columns: ["ID", "Código", "Hora", "Día", "Activo"],
        rows: source.map(p => ({
          ID: p.id,
          Código: p.code,
          Hora: money(p.hourlyRate),
          Día: money(p.dailyRate),
          Activo: p.isActive === false ? "No" : "Sí"
        }))
      },
      vehicles: {
        name: "Vehículos",
        columns: ["ID", "Usuario", "Placa", "Marca", "Modelo", "Color"],
        rows: source.map(v => ({
          ID: v.id,
          Usuario: v.userName || users.find(u => Number(u.id) === Number(v.userId))?.firstName || v.userId,
          Placa: v.plate,
          Marca: v.brand,
          Modelo: v.model,
          Color: v.color
        }))
      },
      reservations: {
        name: "Reservas",
        columns: ["ID", "Usuario", "Parqueo", "Vehículo", "Fecha", "Horario", "Estado", "Total"],
        rows: source.map(r => ({
          ID: r.id,
          Usuario: r.userName || users.find(u => Number(u.id) === Number(r.userId))?.firstName || r.userId,
          Parqueo: r.parkingCode || parkings.find(p => Number(p.id) === Number(r.parkingId))?.code || r.parkingId,
          Vehículo: r.vehiclePlate || vehicles.find(v => Number(v.id) === Number(r.vehicleId))?.plate || r.vehicleId,
          Fecha: dateOnly(r.reservationDate),
          Horario: range(r.startTime, r.endTime),
          Estado: r.status,
          Total: money(r.totalAmount)
        }))
      },
      wallets: {
        name: "Billeteras",
        columns: ["ID", "Usuario", "Balance", "Activo", "Creada", "Actualizada"],
        rows: source.map(w => ({
          ID: w.id,
          Usuario: w.userName || users.find(u => Number(u.id) === Number(w.userId))?.firstName || w.userId,
          Balance: money(w.balance),
          Activo: w.isActive === false ? "No" : "Sí",
          Creada: dateTimeLabel(w.createdAt),
          Actualizada: dateTimeLabel(w.updatedAt)
        }))
      },
      "wallet-transactions": {
        name: "Movimientos de billetera",
        columns: ["ID", "Usuario", "Reserva", "Fecha", "Tipo", "Descripción", "Monto", "Estado"],
        rows: source.map(t => ({
          ID: t.id,
          Usuario: t.userName || users.find(u => Number(u.id) === Number(t.userId))?.firstName || t.userId,
          Reserva: t.reservationId || "-",
          Fecha: dateTimeLabel(t.createdAt),
          Tipo: transactionTypeLabel(t.transactionType),
          Descripción: t.description,
          Monto: money(walletAmountValue(t)),
          Estado: t.status
        }))
      }
    };

    return definitions[type] || definitions.users;
  };

  const activeReport = buildReport(reportType, reportRows);

  const createVehicle = async () => {
    if (!currentUserId) return showToast("Selecciona un usuario.");
    try {
      await api("/vehicles", { method: "POST", body: JSON.stringify({ ...vehicleForm, userId: Number(currentUserId) }) });
      setVehicleForm(emptyVehicle);
      await loadAll();
      showToast("Vehículo guardado.");
    } catch (error) { showToast(error.message); }
  };

  const createReservation = async (data, error) => {
    if (!data) return showToast(error);
    try {
      await api("/reservations", { method: "POST", body: JSON.stringify(data) });
      setModalParking(null);
      await loadAll();
      showToast("Reserva creada. El monto fue descontado de la billetera.");
    } catch (error) { showToast(error.message); }
  };

  const cancelReservation = async (id) => {
    const ok = await askConfirm({
      title: "Cancelar reserva",
      message: "¿Deseas cancelar esta reserva?"
    });
    if (!ok) return;

    try {
      await api(`/reservations/cancel/${id}`, { method: "PUT" });
      await loadAll();
      showToast("Reserva cancelada. El dinero fue devuelto a la billetera.");
    } catch (error) { showToast(error.message); }
  };

  const deleteItem = async (path, label) => {
    const ok = await askConfirm({
      title: "Eliminar registro",
      message: `¿Deseas eliminar ${label}?`
    });
    if (!ok) return;

    try {
      await api(path, { method: "DELETE" });
      await loadAll();
      showToast("Registro eliminado.");
    } catch (error) { showToast(error.message); }
  };

  const createUser = async () => {
    try {
      await api("/users", { method: "POST", body: JSON.stringify(userForm) });
      setUserForm(emptyUser);
      await loadAll();
      showToast("Usuario creado.");
    } catch (error) { showToast(error.message); }
  };

  const createParking = async () => {
    try {
      await api("/parkings", {
        method: "POST",
        body: JSON.stringify({
          code: parkingForm.code,
          hourlyRate: Number(parkingForm.hourlyRate),
          dailyRate: Number(parkingForm.dailyRate)
        })
      });
      setParkingForm(emptyParking);
      await loadAll();
      showToast("Parqueo creado.");
    } catch (error) { showToast(error.message); }
  };

  const createAdminVehicle = async () => {
    if (!adminVehicleForm.userId) return showToast("Selecciona un usuario.");
    try {
      await api("/vehicles", {
        method: "POST",
        body: JSON.stringify({ ...adminVehicleForm, userId: Number(adminVehicleForm.userId) })
      });
      setAdminVehicleForm({ userId: "", plate: "", brand: "", model: "", color: "" });
      await loadAll();
      showToast("Vehículo registrado.");
    } catch (error) { showToast(error.message); }
  };

  const addWalletAmount = async () => {
    if (!walletForm.userId) return showToast("Selecciona un usuario.");
    if (!walletForm.amount || Number(walletForm.amount) <= 0) return showToast("Coloca un monto válido.");

    try {
      await api("/wallets/add-amount", {
        method: "POST",
        body: JSON.stringify({
          userId: Number(walletForm.userId),
          amount: Number(walletForm.amount),
          description: walletForm.description || "Recarga de billetera"
        })
      });

      setWalletForm({ userId: "", amount: "", description: "Recarga de billetera" });
      await loadAll();
      showToast("Recarga realizada correctamente.");
    } catch (error) {
      showToast(error.message);
    }
  };

  const downloadReport = () => {
    const rows = activeReport.rows || [];
    const headers = activeReport.columns || [];
    const csv = [
      headers.join(","),
      ...rows.map(row => headers.map(h => `"${String(row[h] ?? "").replaceAll('"', '""')}"`).join(","))
    ].join("\n");

    const blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = `ParkRD-${activeReport.name}.csv`;
    link.click();
    URL.revokeObjectURL(url);
  };

  if (!mode) return <><Home onEnter={enterMode} /><Toast message={toast} /></>;

  return (
    <>
      <Sidebar mode={mode} view={view} onView={setView} onHome={goHome} />
      <Topbar mode={mode} view={view} />

      <main className="app-shell">
        {mode === "user" && view === "user" && (
          <>
            <Header
              title="Reserva tu espacio de parqueo"
              subtitle="Filtra por fecha y hora. Los espacios se muestran en carrusel horizontal."
              right={<div className="mini-stat"><span>Disponibles</span><strong>{filteredParkings.filter(x => !x.reserved).length}</strong></div>}
            />

            <section className="panel filter-panel">
              <div className="form-grid five">
                <label>Usuario
                  <select value={currentUserId} onChange={e => setCurrentUserId(e.target.value)}>
                    <option value="">Selecciona un usuario</option>
                    {users.map(u => <option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>)}
                  </select>
                </label>
                <label>Fecha<input type="date" value={filterDate} onChange={e => setFilterDate(e.target.value)} /></label>
                <label>Hora inicio
                  <select value={filterStart} onChange={e => setFilterStart(e.target.value)}>
                    {Array.from({ length: 15 }, (_, i) => 7 + i).map(h => (
                      <option key={h} value={`${String(h).padStart(2, "0")}:00`}>{ampm(`${String(h).padStart(2, "0")}:00`)}</option>
                    ))}
                  </select>
                </label>
                <label>Duración
                  <select value={filterDuration} onChange={e => setFilterDuration(e.target.value)}>
                    {[1, 2, 3, 4, 5, 6, 8].map(h => <option key={h} value={h}>{h} hora{h > 1 ? "s" : ""}</option>)}
                  </select>
                </label>
                <label>Mostrar
                  <select value={filterMode} onChange={e => setFilterMode(e.target.value)}>
                    <option value="available">Solo disponibles</option>
                    <option value="all">Todos</option>
                    <option value="reserved">Solo reservados</option>
                  </select>
                </label>
              </div>
            </section>

            <section className="section-title">
              <div><h2>Parqueos</h2><p>Desliza hacia los lados para ver más espacios.</p></div>
            </section>

            <section className="parking-scroller">
              {filteredParkings.map(({ parking, reserved, countDay }) => (
                <article className="parking-card" key={parking.id}>
                  <div className="parking-visual">
                    <span className={reserved ? "badge reserved" : "badge available"}>{reserved ? "Reservado" : "Disponible"}</span>
                    <strong>{parking.code}</strong>
                    <small>{countDay ? `${countDay} reserva${countDay > 1 ? "s" : ""} este día` : "Sin reservas este día"}</small>
                  </div>
                  <div className="parking-body">
                    <div className="rates">
                      <span>Hora <b>{money(parking.hourlyRate)}</b></span>
                      <span>Día <b>{money(parking.dailyRate)}</b></span>
                    </div>
                    <button disabled={reserved} onClick={() => setModalParking(parking)}>{reserved ? "No disponible" : "Reservar"}</button>
                  </div>
                </article>
              ))}
            </section>

            <section className="grid-two">
              <section className="panel">
                <h2>Registrar vehículo</h2>
                <p>Asocia un vehículo al usuario seleccionado.</p>
                <div className="form-grid one">
                  <label>Placa<input value={vehicleForm.plate} onChange={e => setVehicleForm({ ...vehicleForm, plate: e.target.value })} /></label>
                  <label>Marca<input value={vehicleForm.brand} onChange={e => setVehicleForm({ ...vehicleForm, brand: e.target.value })} /></label>
                  <label>Modelo<input value={vehicleForm.model} onChange={e => setVehicleForm({ ...vehicleForm, model: e.target.value })} /></label>
                  <label>Color<input value={vehicleForm.color} onChange={e => setVehicleForm({ ...vehicleForm, color: e.target.value })} /></label>
                </div>
                <button className="btn btn-red full" onClick={createVehicle}>Guardar vehículo</button>
              </section>

              <section className="panel">
                <h2>Mis vehículos</h2>
                <DataTable columns={["ID", "Placa", "Marca", "Modelo", "Color", "Acción"]}>
                  {userVehicles.map(v => (
                    <tr key={v.id}>
                      <td>{v.id}</td><td>{v.plate}</td><td>{v.brand}</td><td>{v.model}</td><td>{v.color}</td>
                      <td><button className="danger" onClick={() => deleteItem(`/vehicles/${v.id}`, "este vehículo")}>Eliminar</button></td>
                    </tr>
                  ))}
                </DataTable>
              </section>
            </section>

            <section className="panel">
              <h2>Mis reservas</h2>
              <DataTable columns={["ID", "Parqueo", "Vehículo", "Fecha", "Horario", "Tipo", "Estado", "Total", "Acción"]}>
                {userReservations.map(r => (
                  <tr key={r.id}>
                    <td>{r.id}</td>
                    <td>{r.parkingCode || parkings.find(p => Number(p.id) === Number(r.parkingId))?.code}</td>
                    <td>{r.vehiclePlate || vehicles.find(v => Number(v.id) === Number(r.vehicleId))?.plate}</td>
                    <td>{dateOnly(r.reservationDate)}</td>
                    <td>{range(r.startTime, r.endTime)}</td>
                    <td>{r.reservationType}</td>
                    <td>{r.status}</td>
                    <td>{money(r.totalAmount)}</td>
                    <td>{r.status === "Reserved" && <button className="warning" onClick={() => cancelReservation(r.id)}>Cancelar</button>}</td>
                  </tr>
                ))}
              </DataTable>
            </section>
          </>
        )}

        {mode === "user" && view === "user-wallet" && (
          <>
            <Header title="Billetera" subtitle="Consulta balance, recargas y pagos realizados en ParkRD." />

            <section className="stats-grid wallet-stats">
              <StatCard label="Balance disponible" value={money(currentUserBalance)} />
              <StatCard label="Movimientos" value={currentUserWalletTransactions.length} />
              <StatCard label="Pagos realizados" value={currentUserWalletTransactions.filter(t => String(t.transactionType || "").toLowerCase().includes("payment")).length} />
              <StatCard label="Devoluciones" value={currentUserWalletTransactions.filter(t => String(t.transactionType || "").toLowerCase().includes("refund")).length} />
            </section>

            <section className="panel">
              <h2>Movimientos</h2>
              <DataTable columns={["ID", "Fecha", "Tipo", "Descripción", "Monto", "Estado"]}>
                {currentUserWalletTransactions.map(t => (
                  <tr key={t.id}>
                    <td>{t.id}</td><td>{dateTimeLabel(t.createdAt)}</td><td>{transactionTypeLabel(t.transactionType)}</td><td>{t.description}</td><td>{money(walletAmountValue(t))}</td><td>{t.status}</td>
                  </tr>
                ))}
              </DataTable>
            </section>
          </>
        )}

        {mode === "admin" && view === "admin" && (
          <>
            <Header title="Panel administrativo" subtitle="Resumen general del sistema." />
            <section className="stats-grid admin-main-stats">
              <StatCard label="Usuarios" value={users.length} />
              <StatCard label="Vehículos" value={vehicles.length} />
              <StatCard label="Parqueos" value={parkings.length} />
              <StatCard label="Reservas" value={reservations.length} />
              <StatCard label="Balance disponible" value={money(walletBalance)} />
            </section>
            <section className="panel">
              <h2>Administración</h2>
              <p>Desde este panel puedes gestionar usuarios, parqueos, vehículos, reservas, billetera y reportes.</p>
              <div className="shortcut-grid admin-shortcuts">
                <button onClick={() => setView("admin-users")}><Icon name="user" />Usuarios</button>
                <button onClick={() => setView("admin-parkings")}><Icon name="parking" />Parqueos</button>
                <button onClick={() => setView("admin-vehicles")}><Icon name="car" />Vehículos</button>
                <button onClick={() => setView("admin-reservations")}><Icon name="calendar" />Reservas</button>
                <button onClick={() => setView("admin-wallet")}><Icon name="wallet" />Billetera</button>
                <button onClick={() => setView("admin-reports")}><Icon name="chart" />Reportes</button>
              </div>
            </section>
          </>
        )}

        {mode === "admin" && view === "admin-users" && (
          <>
            <Header title="Usuarios" subtitle="Crear, editar y eliminar usuarios." />
            <section className="grid-two">
              <section className="panel">
                <h2>Nuevo usuario</h2>
                <div className="form-grid one">
                  <label>Nombre<input value={userForm.firstName} onChange={e => setUserForm({ ...userForm, firstName: e.target.value })} /></label>
                  <label>Apellido<input value={userForm.lastName} onChange={e => setUserForm({ ...userForm, lastName: e.target.value })} /></label>
                  <label>Cédula<input value={userForm.nationalId} onChange={e => setUserForm({ ...userForm, nationalId: e.target.value })} /></label>
                  <label>Email<input value={userForm.email} onChange={e => setUserForm({ ...userForm, email: e.target.value })} /></label>
                  <label>Teléfono<input value={userForm.phoneNumber} onChange={e => setUserForm({ ...userForm, phoneNumber: e.target.value })} /></label>
                </div>
                <button className="btn btn-red full" onClick={createUser}>Guardar usuario</button>
              </section>

              <section className="panel">
                <h2>Usuarios registrados</h2>
                <DataTable columns={["ID", "Nombre", "Cédula", "Email", "Teléfono", "Acción"]}>
                  {users.map(u => (
                    <tr key={u.id}>
                      <td>{u.id}</td><td>{u.firstName} {u.lastName}</td><td>{u.nationalId}</td><td>{u.email}</td><td>{u.phoneNumber}</td>
                      <td><button className="danger" onClick={() => deleteItem(`/users/${u.id}`, "este usuario")}>Eliminar</button></td>
                    </tr>
                  ))}
                </DataTable>
              </section>
            </section>
          </>
        )}

        {mode === "admin" && view === "admin-parkings" && (
          <>
            <Header title="Parqueos" subtitle="Crear, editar y eliminar espacios." />
            <section className="grid-two">
              <section className="panel">
                <h2>Nuevo parqueo</h2>
                <div className="form-grid one">
                  <label>Código<input value={parkingForm.code} onChange={e => setParkingForm({ ...parkingForm, code: e.target.value })} /></label>
                  <label>Tarifa por hora<input type="number" value={parkingForm.hourlyRate} onChange={e => setParkingForm({ ...parkingForm, hourlyRate: e.target.value })} /></label>
                  <label>Tarifa por día<input type="number" value={parkingForm.dailyRate} onChange={e => setParkingForm({ ...parkingForm, dailyRate: e.target.value })} /></label>
                </div>
                <button className="btn btn-red full" onClick={createParking}>Guardar parqueo</button>
              </section>

              <section className="panel">
                <h2>Parqueos registrados</h2>
                <DataTable columns={["ID", "Código", "Hora", "Día", "Estado", "Activo", "Acción"]}>
                  {parkings.map(p => (
                    <tr key={p.id}>
                      <td>{p.id}</td><td>{p.code}</td><td>{money(p.hourlyRate)}</td><td>{money(p.dailyRate)}</td><td>Según horario</td><td>{p.isActive === false ? "No" : "Sí"}</td>
                      <td><button className="danger" onClick={() => deleteItem(`/parkings/${p.id}`, "este parqueo")}>Eliminar</button></td>
                    </tr>
                  ))}
                </DataTable>
              </section>
            </section>
          </>
        )}

        {mode === "admin" && view === "admin-vehicles" && (
          <>
            <Header title="Vehículos" subtitle="Registrar y administrar vehículos de los usuarios." />
            <section className="grid-two">
              <section className="panel">
                <h2>Registrar vehículo</h2>
                <p>Como administrador puedes asociar un vehículo a cualquier usuario.</p>
                <div className="form-grid one">
                  <label>Usuario
                    <select value={adminVehicleForm.userId} onChange={e => setAdminVehicleForm({ ...adminVehicleForm, userId: e.target.value })}>
                      <option value="">Selecciona un usuario</option>
                      {users.map(u => <option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>)}
                    </select>
                  </label>
                  <label>Placa<input value={adminVehicleForm.plate} onChange={e => setAdminVehicleForm({ ...adminVehicleForm, plate: e.target.value })} /></label>
                  <label>Marca<input value={adminVehicleForm.brand} onChange={e => setAdminVehicleForm({ ...adminVehicleForm, brand: e.target.value })} /></label>
                  <label>Modelo<input value={adminVehicleForm.model} onChange={e => setAdminVehicleForm({ ...adminVehicleForm, model: e.target.value })} /></label>
                  <label>Color<input value={adminVehicleForm.color} onChange={e => setAdminVehicleForm({ ...adminVehicleForm, color: e.target.value })} /></label>
                </div>
                <button className="btn btn-red full" onClick={createAdminVehicle}>Registrar vehículo</button>
              </section>

              <section className="panel">
                <h2>Vehículos</h2>
                <DataTable columns={["ID", "Usuario", "Placa", "Marca", "Modelo", "Color", "Acción"]}>
                  {vehicles.map(v => (
                    <tr key={v.id}>
                      <td>{v.id}</td><td>{v.userName || users.find(u => Number(u.id) === Number(v.userId))?.firstName || v.userId}</td>
                      <td>{v.plate}</td><td>{v.brand}</td><td>{v.model}</td><td>{v.color}</td>
                      <td><button className="danger" onClick={() => deleteItem(`/vehicles/${v.id}`, "este vehículo")}>Eliminar</button></td>
                    </tr>
                  ))}
                </DataTable>
              </section>
            </section>
          </>
        )}

        {mode === "admin" && view === "admin-reservations" && (
          <>
            <Header title="Reservas" subtitle="Editar, cancelar o eliminar reservas." />
            <section className="panel">
              <h2>Reservas registradas</h2>
              <DataTable columns={["ID", "Usuario", "Parqueo", "Vehículo", "Fecha", "Horario", "Tipo", "Estado", "Total", "Acción"]}>
                {reservations.map(r => (
                  <tr key={r.id}>
                    <td>{r.id}</td>
                    <td>{r.userName || users.find(u => Number(u.id) === Number(r.userId))?.firstName || r.userId}</td>
                    <td>{r.parkingCode || parkings.find(p => Number(p.id) === Number(r.parkingId))?.code}</td>
                    <td>{r.vehiclePlate || vehicles.find(v => Number(v.id) === Number(r.vehicleId))?.plate}</td>
                    <td>{dateOnly(r.reservationDate)}</td><td>{range(r.startTime, r.endTime)}</td><td>{r.reservationType}</td><td>{r.status}</td><td>{money(r.totalAmount)}</td>
                    <td className="table-actions">
                      {r.status === "Reserved" && <button className="warning" onClick={() => cancelReservation(r.id)}>Cancelar</button>}
                      <button className="danger" onClick={() => deleteItem(`/reservations/${r.id}`, "esta reserva")}>Eliminar</button>
                    </td>
                  </tr>
                ))}
              </DataTable>
            </section>
          </>
        )}
        {mode === "admin" && view === "admin-wallet" && (
          <>
            <Header title="Billetera" subtitle="Agregar monto a usuarios y consultar movimientos." />

            <section className="stats-grid wallet-stats">
              <StatCard label="Balance total" value={money(walletBalance)} />
              <StatCard label="Movimientos" value={walletTransactions.length} />
              <StatCard label="Recargas" value={walletTransactions.filter(t => String(t.transactionType || "").toLowerCase().includes("recharge")).length} />
              <StatCard label="Pagos" value={walletTransactions.filter(t => String(t.transactionType || "").toLowerCase().includes("payment")).length} />
            </section>

            <section className="grid-two">
              <section className="panel">
                <h2>Agregar monto</h2>
                <p>El monto se registra directamente en la billetera del usuario y queda en el historial de movimientos.</p>
                <div className="form-grid one">
                  <label>Usuario
                    <select value={walletForm.userId} onChange={e => setWalletForm({ ...walletForm, userId: e.target.value })}>
                      <option value="">Selecciona un usuario</option>
                      {users.map(u => <option key={u.id} value={u.id}>{u.firstName} {u.lastName}</option>)}
                    </select>
                  </label>
                  <label>Monto
                    <input type="number" value={walletForm.amount} onChange={e => setWalletForm({ ...walletForm, amount: e.target.value })} placeholder="Ej: 500" />
                  </label>
                  <label>Descripción
                    <input value={walletForm.description} onChange={e => setWalletForm({ ...walletForm, description: e.target.value })} />
                  </label>
                </div>
                <button className="btn btn-red full" onClick={addWalletAmount}>Agregar monto</button>
              </section>

              <section className="panel">
                <h2>Movimientos de billetera</h2>
                <DataTable columns={["ID", "Usuario", "Fecha", "Tipo", "Descripción", "Monto", "Estado"]}>
                  {walletTransactions.map(t => (
                    <tr key={t.id}>
                      <td>{t.id}</td><td>{t.userName}</td><td>{dateTimeLabel(t.createdAt)}</td><td>{transactionTypeLabel(t.transactionType)}</td><td>{t.description}</td><td>{money(walletAmountValue(t))}</td><td>{t.status}</td>
                    </tr>
                  ))}
                </DataTable>
              </section>
            </section>
          </>
        )}

        {mode === "admin" && view === "admin-reports" && (
          <>
            <Header title="Reportes" subtitle="Genera reportes desde los endpoints del backend." />

            <section className="panel report-panel">
              <div className="report-toolbar">
                <label>Tipo de reporte
                  <select value={reportType} onChange={e => setReportType(e.target.value)}>
                    <option value="users">Usuarios</option>
                    <option value="parkings">Parqueos</option>
                    <option value="vehicles">Vehículos</option>
                    <option value="reservations">Reservas</option>
                    <option value="wallets">Billeteras</option>
                    <option value="wallet-transactions">Movimientos de billetera</option>
                  </select>
                </label>
                <button className="btn btn-blue" onClick={downloadReport}>Descargar CSV</button>
              </div>

              <DataTable columns={activeReport.columns}>
                {reportLoading ? (
                  <tr><td colSpan={activeReport.columns.length}>Cargando reporte...</td></tr>
                ) : activeReport.rows.map((row, index) => (
                  <tr key={index}>
                    {activeReport.columns.map(col => <td key={col}>{row[col]}</td>)}
                  </tr>
                ))}
              </DataTable>
            </section>
          </>
        )}

      </main>

      <ReservationModal open={!!modalParking} parking={modalParking} vehicles={vehicles} currentUserId={currentUserId} onClose={() => setModalParking(null)} onCreate={createReservation} />
      <ConfirmDialog data={confirmData} onCancel={() => closeConfirm(false)} onConfirm={() => closeConfirm(true)} />
      <Toast message={toast} />
    </>
  );
}
