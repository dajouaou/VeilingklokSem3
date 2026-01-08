import { useEffect, useState, useContext } from "react";
import { AuthContext } from "../features/auth/AuthContext";
import { fetchAlleVeilingen } from "../features/veiling/api/veilingApi";
import Navbar from "../shared/components/Navbar";
import Footer from "../shared/components/Footer";

function statusLabel(status) {
    switch (status) {
        case 0: return "Niet gestart";
        case 1: return "Gestart";
        case 2: return "Gepauzeerd";
        case 3: return "Afgesloten";
        default: return "Onbekend";
    }
}

function statusBadge(status) {
    switch (status) {
        case 1: return "bg-success";
        case 2: return "bg-warning text-dark";
        case 3: return "bg-secondary";
        default: return "bg-light text-dark";
    }
}

export default function Veilingen() {
    const { token } = useContext(AuthContext);

    const [veilingen, setVeilingen] = useState([]);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!token) return;

        setLoading(true);
        fetchAlleVeilingen(token)
            .then(setVeilingen)
            .catch(err => {
                setError(err?.message || "Kon veilingen niet laden");
            })
            .finally(() => setLoading(false));
    }, [token]);

    return (
        <>
            <Navbar />

            <div className="container py-5">
                <h2 className="fw-bold mb-4">Alle veilingen</h2>

                {loading && <p>Veilingen laden…</p>}

                {error && (
                    <div className="alert alert-danger">
                        {error}
                    </div>
                )}

                {!loading && veilingen.length === 0 && (
                    <p className="text-muted">Geen veilingen gevonden.</p>
                )}

                <div className="row">
                    {veilingen.map(v => (
                        <div className="col-md-6 col-lg-4" key={v.id}>
                            <div className="card mb-3">
                                <div className="card-body">
                                    <div className="d-flex justify-content-between align-items-center mb-2">
                                        <strong>Veiling #{v.id}</strong>
                                        <span className={`badge ${statusBadge(v.status)}`}>
                                            {statusLabel(v.status)}
                                        </span>
                                    </div>

                                    <div>Datum: {v.veildatum ?? "-"}</div>
                                    <div>Starttijd: {v.startTijd ?? "-"}</div>

                                    {v.aantalProducten != null && (
                                        <div>Producten: {v.aantalProducten}</div>
                                    )}

                                    {v.huidigProductId && (
                                        <div className="mt-2 text-success">
                                            Actief product ID: {v.huidigProductId}
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </div>

            <Footer />
        </>
    );
}
