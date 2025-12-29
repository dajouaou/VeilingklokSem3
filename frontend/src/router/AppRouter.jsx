import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import AuthPage from "../features/auth/pages/AuthPage";
import VeilingmeesterDashboard from "../features/VMDashboard/pages/VeilingmeesterDashboard.jsx";

function getToken() {
    return localStorage.getItem("token") || "";
}

function decodePayload(token) {
    try {
        const part = String(token || "").split(".")[1];
        if (!part) return null;
        const base64 = part.replace(/-/g, "+").replace(/_/g, "/");
        const padded = base64 + "=".repeat((4 - (base64.length % 4)) % 4);
        return JSON.parse(atob(padded));
    } catch {
        return null;
    }
}

function getRoleFromToken(token) {
    const payload = decodePayload(token);
    if (!payload) return "";
    const c =
        payload.role ||
        payload.Role ||
        payload.rol ||
        payload.Rol ||
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"];
    if (typeof c === "string") return c.trim();
    if (Array.isArray(c) && typeof c[0] === "string") return c[0].trim();
    return "";
}

function routeForRole(roleRaw) {
    const role = String(roleRaw || "").toLowerCase();
    if (role === "vm" || role.includes("veilingmeester")) return "/vm";
    if (role.includes("aanvoerder")) return "/aanvoerder";
    if (role.includes("koper")) return "/veiling";
    return "/login";
}

function RequireAuth({ children }) {
    const token = getToken();
    if (!token) return <Navigate to="/login" replace />;
    return children;
}

function RequireRole({ allow, children }) {
    const token = getToken();
    if (!token) return <Navigate to="/login" replace />;
    const role = String(getRoleFromToken(token)).toLowerCase();
    const ok = allow.some((a) => role === a || role.includes(a));
    if (!ok) return <Navigate to={routeForRole(role)} replace />;
    return children;
}

function HomeRedirect() {
    const token = getToken();
    if (!token) return <Navigate to="/login" replace />;
    const role = getRoleFromToken(token);
    return <Navigate to={routeForRole(role)} replace />;
}

function LoginGate() {
    const token = getToken();
    if (token) {
        const role = getRoleFromToken(token);
        return <Navigate to={routeForRole(role)} replace />;
    }
    return <AuthPage />;
}

function Placeholder({ title }) {
    return (
        <div className="vm-shell">
            <main className="vm-main">
                <div className="vm-content">
                    <div className="vm-panel">
                        <div className="vm-panel__header">
                            <div className="vm-panel__title">{title}</div>
                            <div className="vm-panel__hint">Binnenkort</div>
                        </div>
                        <div className="vm-empty">
                            <div className="vm-empty__title">Nog niet beschikbaar</div>
                            <div className="vm-empty__text">Deze pagina bouw je later.</div>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
}

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<HomeRedirect />} />
                <Route path="/login" element={<LoginGate />} />

                <Route
                    path="/vm"
                    element={
                        <RequireRole allow={["vm", "veilingmeester"]}>
                            <VeilingmeesterDashboard />
                        </RequireRole>
                    }
                />

                <Route
                    path="/aanvoerder"
                    element={
                        <RequireAuth>
                            <Placeholder title="Aanvoerder dashboard" />
                        </RequireAuth>
                    }
                />

                <Route
                    path="/veiling"
                    element={
                        <RequireAuth>
                            <Placeholder title="Veiling pagina (koper)" />
                        </RequireAuth>
                    }
                />

                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </BrowserRouter>
    );
}
