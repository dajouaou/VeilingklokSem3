import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "../features/auth/Login.jsx";
import Register from "../features/auth/Register.jsx";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Login />} />   {/* Default route */}
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
            </Routes>
        </BrowserRouter>
    );
}
