import { BrowserRouter, Routes, Route } from "react-router-dom";
import VeilingmeesterDashboard from "frontend/features/veilingmeesterDashboard/pages/VeilingmeesterDashboard";
import Login from "../features/auth/Login";
import Register from "../features/auth/Register";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                {/* Homepage (tijdelijk dashboard) */}
                <Route path="/" element={<VeilingmeesterDashboard />} />

                {/* Auth routes */}
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
            </Routes>
        </BrowserRouter>
    );
}
