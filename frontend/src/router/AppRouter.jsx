// src/router/AppRouter.jsx
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import HomePage from "../features/home/HomePage.jsx";
import VeilingPage from "../features/veiling/VeilingPage.jsx";
import VMDashboard from "../features/VM/pages/VMDashboard.jsx";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<HomePage />} />

                {/* redirect /veiling → /veiling/1 (of een andere default) */}
                <Route path="/veiling" element={<Navigate to="/veiling/1" replace />} />

                {/* publieke veiling met id uit URL */}
                <Route path="/veiling/:veilingId" element={<VeilingPage />} />

                {/* VM dashboard */}
                <Route path="/vm" element={<Navigate to="/vm/1" replace />} />
                <Route path="/vm/:veilingId" element={<VMDashboard />} />

                {/* fallback */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </BrowserRouter>
    );
}
