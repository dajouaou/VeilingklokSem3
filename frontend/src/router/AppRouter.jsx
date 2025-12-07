// src/router/AppRouter.jsx

import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

// pagina's
import HomePage from "../pages/HomePage";
import VMDashboard from "../pages/VMDashboard";
import VeilingPage from "../pages/VeilingPage";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                {/* Home / landingspagina */}
                <Route path="/" element={<HomePage />} />

                {/* Veilingpagina voor kopers */}
                <Route path="/veiling" element={<VeilingPage />} />

                {/* Dashboard voor veilingmeester */}
                <Route path="/vm" element={<VMDashboard />} />

                {/* Fallback: onbekende URL → terug naar home */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </BrowserRouter>
    );
}
