import { createContext, useState, useEffect } from "react";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [token, setToken] = useState(localStorage.getItem("token"));
    const [role, setRole] = useState(localStorage.getItem("role"));

    function login(token, role) {
        setToken(token);
        setRole(role);
        localStorage.setItem("token", token);
        localStorage.setItem("role", role);
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
