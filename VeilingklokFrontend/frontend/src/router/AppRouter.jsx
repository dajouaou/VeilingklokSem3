import { BrowserRouter, Routes, Route } from "react-router-dom";
import VeilingmeesterDashboard from "../features/veilingmeesterDashboard/pages/VeilingmeesterDashboard.jsx";
import Login from "../features/auth/api/Login.jsx";
import Register from "../features/auth/api/Register.jsx";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<VeilingmeesterDashboard />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
            </Routes>
        </BrowserRouter>
    );
}
