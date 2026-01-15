import { Routes, Route } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";

import Login from "../features/auth/Login.jsx";
import Register from "../features/auth/Register.jsx";
import HomePage from "../features/home/HomePage.jsx";
import AanvoerderDashboard from "../features/aanvoerderDashboard/AanvoerderDashboard";
import VeilingmeesterDashboard from "../features/veilingmeesterDashboard/VeilingmeesterDashboard";
import PlanVeiling from "../features/veilingmeesterDashboard/PlanVeiling";
import GeplandeVeilingen from "../features/veilingmeesterDashboard/components/GeplandeVeilingen";
import Archief from "../features/veilingmeesterDashboard/Archief";




import ActueelBod from "../features/Actueelbod.jsx";

export default function AppRouter() {
    return (
        <Routes>

            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            <Route
                path="/actueelbod"
                element={
                    <ProtectedRoute allowedRoles={["Koper"]}>
                        <ActueelBod />
                    </ProtectedRoute>
                }
            />


            <Route path="/" element={<HomePage />} />

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

            <Route
                path="/veilingmeester/plan"
                element={
                    <ProtectedRoute allowedRoles={["Veilingmeester"]}>
                        <PlanVeiling />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/veilingmeester/gepland"
                element={
                    <ProtectedRoute allowedRoles={["Veilingmeester"]}>
                        <GeplandeVeilingen />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/veilingmeester/archief"
                element={
                    <ProtectedRoute allowedRoles={["Veilingmeester"]}>
                        <Archief />
                    </ProtectedRoute>
                }
            />



        </Routes>
    );
}
