import { useContext } from "react";
import { AuthContext } from "../features/auth/AuthContext";
import { Navigate } from "react-router-dom";

export default function ProtectedRoute({ allowedRoles, children }) {
    const { token, role } = useContext(AuthContext);

    if (!token) {
        return <Navigate to="/login" replace />;
    }

    if (!allowedRoles.includes(role)) {
        return <Navigate to="/" replace />;
    }

    return children;
}
