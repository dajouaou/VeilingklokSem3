import { createContext, useEffect, useState } from "react";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [token, setToken] = useState(undefined); 
    const [role, setRole] = useState(undefined);

    useEffect(() => {
        const t = localStorage.getItem("token");
        const r = localStorage.getItem("role");
        setToken(t || null);
        setRole(r || null);
    }, []);

    function login(newToken, newRole) {
        setToken(newToken);
        setRole(newRole);
        localStorage.setItem("token", newToken);
        localStorage.setItem("role", newRole);
    }

    function logout() {
        setToken(null);
        setRole(null);
        localStorage.removeItem("token");
        localStorage.removeItem("role");
    }

    return (
        <AuthContext.Provider value={{ token, role, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}
