import { useContext } from "react";
import { Navigate } from "react-router-dom";
import { AuthContext } from "../features/auth/AuthContext";

export default function ProtectedRoute({ allowedRoles, children }) {
    const { token, role } = useContext(AuthContext);

    if (token === undefined || role === undefined) {
        return <div className="container py-5">Authenticatie wordt gecontroleerd…</div>;
    }

    // Niet ingelogd
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    // Rol niet toegestaan
    if (!allowedRoles.includes(role)) {
        return <Navigate to="/" replace />;
    }

    return children;
}
