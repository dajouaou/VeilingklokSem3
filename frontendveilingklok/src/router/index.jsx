import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import AppRouter from "./AppRouter";
import { AuthProvider } from "./AuthContext";

ReactDOM.createRoot(document.getElementById("root")).render(
    <AuthProvider>
        <BrowserRouter>
            <AppRouter />
        </BrowserRouter>
    </AuthProvider>
);
