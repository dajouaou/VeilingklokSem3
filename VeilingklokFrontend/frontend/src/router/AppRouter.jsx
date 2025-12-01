import { BrowserRouter, Routes, Route } from "react-router-dom";
import VeilingmeesterDashboard from "../pages/VeilingmeesterDashboard";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                {/* Default route */}
                <Route path="/" element={<VeilingmeesterDashboard />} />

                {/* Add more pages later */}
                {/* <Route path="/login" element={<LoginPage />} /> */}
                {/* <Route path="/veiling/:id" element={<VeilingPage />} /> */}
            </Routes>
        </BrowserRouter>
    );
}
