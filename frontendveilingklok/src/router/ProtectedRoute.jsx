import { useContext } from "react";
import { AuthContext } from "../features/auth/AuthContext";
import { Navigate } from "react-router-dom";

export default function ProtectedRoute({ allowedRoles, children }) {
    const { token, role } = useContext(AuthContext);

    // Auth laden (token = null)
    if (token === null) {
        return <div>Authenticatie wordt gecontroleerd...</div>;
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
