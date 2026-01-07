import { useEffect, useRef } from "react";
import { NavLink, Link } from "react-router-dom";
import logo from "../../Images/royal floraholland logo new.png";

export default function Navbar() {
    const collapseRef = useRef(null);

    // Scroll shadow effect
    useEffect(() => {
        const onScroll = () => {
            const el = document.querySelector(".navbar.nav-elevated");
            if (!el) return;
            el.classList.toggle("scrolled", window.scrollY > 10);
        };

        onScroll();
        window.addEventListener("scroll", onScroll);
        return () => window.removeEventListener("scroll", onScroll);
    }, []);

    // Close mobile menu after clicking a link
    const closeMobileMenu = () => {
        const el = collapseRef.current;
        if (!el) return;

        // Bootstrap collapse instance (works if Bootstrap JS is loaded)
        const bsCollapse = window.bootstrap?.Collapse?.getOrCreateInstance(el);
        if (bsCollapse) bsCollapse.hide();
    };

    const navLinkClass = ({ isActive }) =>
        `nav-link ${isActive ? "active" : ""}`;

    return (
        <nav className="navbar navbar-expand-lg bg-white sticky-top nav-elevated py-2">
            <div className="container">
                {/* LOGO */}
                <Link className="navbar-brand d-flex align-items-center" to="/" aria-label="Ga naar home">
                    <img src={logo} alt="Royal FloraHolland" className="header-logo" />
                </Link>

                <button
                    className="navbar-toggler shadow-none border-0"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#mainNav"
                    aria-controls="mainNav"
                    aria-expanded="false"
                    aria-label="Menu"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>

                <div id="mainNav" className="collapse navbar-collapse" ref={collapseRef}>
                    <ul className="navbar-nav ms-auto align-items-lg-center gap-lg-2">
                        <li className="nav-item">
                            <NavLink className={navLinkClass} to="/actueelbod" onClick={closeMobileMenu}>
                                Actueel bod
                            </NavLink>
                        </li>

                        <li className="nav-item">
                            <NavLink className={navLinkClass} to="/veiling" onClick={closeMobileMenu}>
                                Veiling
                            </NavLink>
                        </li>

                        <li className="nav-item ms-lg-2">
                            <Link className="btn btn-login rounded-pill px-3" to="/login" onClick={closeMobileMenu}>
                                <i className="bi bi-box-arrow-in-right me-1"></i> Login
                            </Link>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    );
}
