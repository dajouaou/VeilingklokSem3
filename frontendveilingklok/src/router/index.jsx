import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<div>Home werkt!</div>} />
            </Routes>
        </BrowserRouter>
    );
}
