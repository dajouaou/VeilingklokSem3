import { Routes, Route } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";

import Login from "../features/auth/Login.jsx";
import Register from "../features/auth/Register.jsx";
import HomePage from "../features/home/HomePage.jsx";
import AanvoerderDashboard from "../features/aanvoerderDashboard/AanvoerderDashboard";
import VeilingmeesterDashboard from "../features/veilingmeesterDashboard/VeilingmeesterDashboard";

export default function AppRouter() {
    return (
        <Routes>


            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            <Route
                path="/"
                element={
                    <ProtectedRoute allowedRoles={["Koper"]}>
                        <HomePage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/aanvoerder"
                element={
                    <ProtectedRoute allowedRoles={["Aanvoerder"]}>
                        <AanvoerderDashboard />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/veilingmeester"
                element={
                    <ProtectedRoute allowedRoles={["Veilingmeester"]}>
                        <VeilingmeesterDashboard />
                    </ProtectedRoute>
                }
            />

        </Routes>
    );
}
