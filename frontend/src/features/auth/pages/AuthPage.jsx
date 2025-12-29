import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { login, register } from "../api/authApi";

function routeForRole(roleRaw) {
    const role = String(roleRaw || "").toLowerCase();
    if (role === "vm" || role.includes("veilingmeester")) return "/vm";
    if (role.includes("aanvoerder")) return "/aanvoerder";
    if (role.includes("koper")) return "/veiling";
    return "/vm";
}

function normalizeError(msg) {
    const m = String(msg || "");
    const lower = m.toLowerCase();
    if (lower.includes("ongeldige login")) return "Onjuiste e-mail of wachtwoord.";
    if (lower.includes("gebruiker bestaat al")) return "Dit e-mailadres is al in gebruik.";
    if (lower.includes("ongeldige rol")) return "Kies een geldige rol.";
    return "Er ging iets mis. Probeer opnieuw.";
}

export default function AuthPage() {
    const nav = useNavigate();
    const [tab, setTab] = useState("login");

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const [voornaam, setVoornaam] = useState("");
    const [achternaam, setAchternaam] = useState("");
    const [rol, setRol] = useState("Koper");

    const [busy, setBusy] = useState(false);
    const [error, setError] = useState("");

    const title = useMemo(() => (tab === "login" ? "Inloggen" : "Account maken"), [tab]);

    async function onSubmit(e) {
        e.preventDefault();
        setError("");
        setBusy(true);

        try {
            const result =
                tab === "login"
                    ? await login(email.trim(), password)
                    : await register({
                        email: email.trim(),
                        password,
                        voornaam: voornaam.trim(),
                        achternaam: achternaam.trim(),
                        rol,
                    });

            localStorage.setItem("token", result.token);
            localStorage.setItem("role", result.role);

            nav(routeForRole(result.role), { replace: true });
        } catch (e2) {
            setError(normalizeError(e2.message));
        } finally {
            setBusy(false);
        }
    }

    return (
        <div className="auth-shell">
            <div className="auth-card">
                <div className="auth-head">
                    <div className="auth-brand">
                        <div className="auth-brand__title">Veilingklok</div>
                        <div className="auth-brand__subtitle">Beveiligde toegang</div>
                    </div>

                    <div className="auth-tabs" role="tablist">
                        <button
                            type="button"
                            className={`auth-tab ${tab === "login" ? "is-active" : ""}`}
                            onClick={() => setTab("login")}
                        >
                            Inloggen
                        </button>
                        <button
                            type="button"
                            className={`auth-tab ${tab === "register" ? "is-active" : ""}`}
                            onClick={() => setTab("register")}
                        >
                            Account maken
                        </button>
                    </div>
                </div>

                <div className="auth-body">
                    <h1 className="auth-title">{title}</h1>

                    {error && (
                        <div className="auth-alert">
                            <div className="auth-alert__title">Let op</div>
                            <div className="auth-alert__text">{error}</div>
                        </div>
                    )}

                    <form className="auth-form" onSubmit={onSubmit}>
                        {tab === "register" && (
                            <div className="auth-grid2">
                                <input className="auth-input" placeholder="Voornaam" value={voornaam} onChange={(e) => setVoornaam(e.target.value)} />
                                <input className="auth-input" placeholder="Achternaam" value={achternaam} onChange={(e) => setAchternaam(e.target.value)} />
                            </div>
                        )}

                        <input className="auth-input" type="email" placeholder="E-mail" value={email} onChange={(e) => setEmail(e.target.value)} />
                        <input className="auth-input" type="password" placeholder="Wachtwoord" value={password} onChange={(e) => setPassword(e.target.value)} />

                        {tab === "register" && (
                            <select className="auth-input" value={rol} onChange={(e) => setRol(e.target.value)}>
                                <option value="Koper">Koper</option>
                                <option value="Aanvoerder">Aanvoerder</option>
                                <option value="VM">Veilingmeester</option>
                            </select>
                        )}

                        <button className="auth-btn" disabled={busy}>
                            {tab === "login" ? "Inloggen" : "Account maken"}
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
}
