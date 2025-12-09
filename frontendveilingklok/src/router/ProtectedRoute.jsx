import { useContext } from "react";
import { AuthContext } from "../features/auth/AuthContext";
import { Navigate } from "react-router-dom";

export default function ProtectedRoute({ allowedRoles, children }) {
    const { token, role } = useContext(AuthContext);

    // Wachten totdat auth is geladen (null = nog niet bekend)
    if (token === null) {
        return null; // laad nog niets
    }

    // niet ingelogd → naar login
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    // verkeerd role → terug naar home
    if (!allowedRoles.includes(role)) {
        return <Navigate to="/" replace />;
    }

    return children;
}
