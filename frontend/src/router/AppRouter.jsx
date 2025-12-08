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
                <Route path="/veiling" element={<VeilingPage />} />

                <Route path="/vm" element={<Navigate to="/vm/1" replace />} />

                <Route path="/vm/:veilingId" element={<VMDashboard />} />

                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </BrowserRouter>
    );
}
