import { createContext, useEffect, useState } from "react";

export const AuthContext = createContext();

function parseJwt(token) {
    try {
        const base64Url = token.split(".")[1];
        const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split("")
                .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
                .join("")
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
}

export function AuthProvider({ children }) {
    const [token, setToken] = useState(undefined);
    const [role, setRole] = useState(undefined);
    const [naam, setNaam] = useState(undefined);

    useEffect(() => {
        const t = localStorage.getItem("token");
        const r = localStorage.getItem("role");
        const n = localStorage.getItem("naam");
        setToken(t || null);
        setRole(r || null);
        setNaam(n || null);
    }, []);

    function login(newToken, newRole) {
        const payload = parseJwt(newToken);
        const displayName =
            payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
            "";

        setToken(newToken);
        setRole(newRole);
        setNaam(displayName);

        localStorage.setItem("token", newToken);
        localStorage.setItem("role", newRole);
        localStorage.setItem("naam", displayName);
    }

    function logout() {
        setToken(null);
        setRole(null);
        setNaam(null);
        localStorage.removeItem("token");
        localStorage.removeItem("role");
        localStorage.removeItem("naam");
    }

    return (
        <AuthContext.Provider value={{ token, role, naam, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}
