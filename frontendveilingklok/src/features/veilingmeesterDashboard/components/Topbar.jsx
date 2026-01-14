import { useContext, useState, useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../../auth/AuthContext";

export default function Topbar({ title, veiling, onMenuClick }) {
    const { logout, naam, role, token } = useContext(AuthContext);
    const navigate = useNavigate();

    const [open, setOpen] = useState(false);
    const dropdownRef = useRef(null);

    const isVeilingmeester = token && role === "Veilingmeester";

    function handleLogout() {
        logout();
        navigate("/login", { replace: true });
    }

    // sluit dropdown als je buiten klikt
    useEffect(() => {
        function handleClickOutside(e) {
            if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
                setOpen(false);
            }
        }
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    return (
        <header className="vm-top">
            <div className="vm-top-hero">
                <div className="vm-top-left">
                    <button
                        type="button"
                        className="vm-menu"
                        onClick={onMenuClick}
                        aria-label="Open menu"
                    >
                        Menu
                    </button>

                    <div className="vm-top-titles">
                        <div className="vm-top-kicker">Dashboard</div>
                        <h1 className="vm-top-title">{title}</h1>
                        <p className="vm-top-sub">
                            Beheer de klok, wachtrij en biedingen.
                        </p>
                    </div>
                </div>

                <div className="vm-top-right">
                    {veiling && (
                        <div className="vm-pill">
                            <span className="vm-pill-dot" />
                            Veiling #{veiling.id}
                        </div>
                    )}

                    {isVeilingmeester && (
                        <div className="vm-profile" ref={dropdownRef}>
                            <button
                                type="button"
                                className="vm-profile-btn"
                                onClick={() => setOpen(o => !o)}
                                aria-haspopup="menu"
                                aria-expanded={open}
                            >
                                <div className="vm-user-avatar">
                                    {naam?.charAt(0)?.toUpperCase() || "V"}
                                </div>
                                <span className="vm-user-name">{naam}</span>
                            </button>

                            {open && (
                                <div className="vm-profile-dropdown">
                                    <div className="vm-profile-info">
                                        Ingelogd als<br />
                                        <strong>{naam}</strong>
                                    </div>

                                    <button
                                        type="button"
                                        className="vm-profile-logout"
                                        onClick={handleLogout}
                                    >
                                        Uitloggen
                                    </button>
                                </div>
                            )}
                        </div>
                    )}
                </div>
            </div>
        </header>
    );
}
