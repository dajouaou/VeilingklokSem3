import { BrowserRouter, Routes, Route } from "react-router-dom";
import VMDashboard from "../features/VM/pages/VMDashboard";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<VMDashboard />} />
            </Routes>
        </BrowserRouter>
    );
}
