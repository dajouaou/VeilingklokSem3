// src/app/AppLayout.jsx
import { Outlet, useNavigate } from "react-router-dom";

function getToken() {
    return localStorage.getItem("token") || "";
}

export default function AppLayout() {
    const navigate = useNavigate();
    const token = getToken();

    function logout() {
        localStorage.removeItem("token");
        navigate("/", { replace: true });
    }

    if (!token) {
        return <Outlet />;
    }

    return (
        <>
            <header className="app-header">
                <div className="app-header__brand">Digitale Veilingklok</div>
                <button className="app-header__logout" onClick={logout}>
                    Uitloggen
                </button>
            </header>

            <Outlet />
        </>
    );
}
