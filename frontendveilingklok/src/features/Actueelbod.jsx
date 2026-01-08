import { useContext, useEffect, useMemo, useState } from "react";
import Navbar from "../shared/components/Navbar";
import Footer from "../shared/components/Footer";
import { AuthContext } from "./auth/AuthContext";
import useLiveVeiling from "./veilingmeesterDashboard/hooks/useLiveVeiling";
import { getPublicActieveVeiling } from "./veiling/api/veilingPublicApi";
import PrijsHistorieModal from "../shared/components/PrijsHistorieModal";

const API_BASE = "https://localhost:56418";

export default function ActueelBod() {
    const { token, role } = useContext(AuthContext);

    const [veilingId, setVeilingId] = useState(null);
    const [initLot, setInitLot] = useState(null);
    const [wachtrij, setWachtrij] = useState([]);
    const [loadingInit, setLoadingInit] = useState(true);
    const [error, setError] = useState("");

    const [aantal, setAantal] = useState(0);
    const [showHistorie, setShowHistorie] = useState(false);

    useEffect(() => {
        let alive = true;

        async function load() {
            try {
                const actief = await getPublicActieveVeiling();
                if (!alive) return;

                setVeilingId(actief?.id ?? null);
                setInitLot(actief?.huidigProduct ?? null);
                setWachtrij(actief?.wachtrij ?? []);
            } catch {
                setError("Kan actieve veiling niet laden");
            } finally {
                if (alive) setLoadingInit(false);
            }
        }

        load();
        const interval = setInterval(load, 3000);

        return () => {
            alive = false;
            clearInterval(interval);
        };
    }, []);

    const { lot: liveLot, loading: liveLoading } = useLiveVeiling(token, veilingId);
    const lot = liveLot ?? initLot;

    useEffect(() => {
        setAantal(0);
    }, [lot?.veilingProductId]);

    const currentPrice = lot?.huidigePrijs ?? 0;
    const maxAantal = useMemo(
        () => lot?.resterendeHoeveelheid ?? 0,
        [lot]
    );

    async function koop() {
        if (!token || role !== "Koper") return;
        if (!veilingId || !lot?.veilingProductId) return;

        const koopAantal = Number(aantal) <= 0 ? 0 : Number(aantal);

        if (koopAantal > maxAantal) {
            alert(`Aantal is te hoog. Max is ${maxAantal}.`);
            return;
        }

        try {
            const res = await fetch(`${API_BASE}/api/bod/${veilingId}`, {
                method: "POST",
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    veilingProductId: lot.veilingProductId,
                    prijs: currentPrice,
                    aantal: koopAantal,
                }),
            });

            if (!res.ok) throw new Error("Kopen mislukt");

        } catch (e) {
            alert(e.message);
        }
    }

    if (loadingInit || liveLoading) {
        return (
            <>
                <Navbar />
                <div className="container py-5">Veiling laden…</div>
                <Footer />
            </>
        );
    }

    if (!veilingId || !lot) {
        return (
            <>
                <Navbar />
                <div className="container py-5">Geen actieve veiling</div>
                <Footer />
            </>
        );
    }

    return (
        <>
            <Navbar />

            <div className="container py-5">
                <h2 className="fw-bold mb-4">Actueel bod</h2>

                {/* HUIDIG PRODUCT */}
                <div className="row">
                    <div className="col-md-6">
                        <img
                            src={lot.fotoUrl || "/images/bloemen.jpg"}
                            alt={lot.soort}
                            className="img-fluid rounded"
                        />
                    </div>

                    <div className="col-md-6">
                        <h4>{lot.soort}</h4>
                        <small>Resterend: {lot.resterendeHoeveelheid}</small>

                        <h5 className="mt-4">
                            Huidige prijs: €{currentPrice.toFixed(2)}
                        </h5>

                        {role === "Koper" && (
                            <>
                                <div className="mt-3">
                                    <label className="form-label">
                                        Aantal (0 = alles)
                                    </label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        min="0"
                                        max={maxAantal}
                                        value={aantal}
                                        onChange={(e) => setAantal(e.target.value)}
                                    />
                                </div>

                                <button
                                    className="btn btn-dark mt-3"
                                    onClick={koop}
                                >
                                    Koop voor deze prijs
                                </button>
                            </>
                        )}
                    </div>
                </div>

                //volgende veilingen
                {wachtrij.length > 0 && (
                    <div className="mt-5">
                        <h4 className="fw-bold mb-3">VOLGENDE PRODUCTEN</h4>

                        <div className="row">
                            {wachtrij.map((item) => (
                                <div
                                    key={item.veilingProductId}
                                    className="col-md-4 mb-3"
                                >
                                    <div className="card h-100">
                                        <img
                                            src={item.fotoUrl || "/images/bloemen.jpg"}
                                            className="card-img-top"
                                            alt={item.soort}
                                        />
                                        <div className="card-body">
                                            <h6 className="mb-1">
                                                #{item.volgorde} – {item.soort}
                                            </h6>
                                            <small className="text-muted">
                                                Resterend: {item.resterendeHoeveelheid}
                                            </small>
                                            <br />
                                            <small className="text-muted">
                                                Startprijs: €{item.maximumPrijs.toFixed(2)}
                                            </small>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                )}
            </div>

            <Footer />
        </>
    );
}
