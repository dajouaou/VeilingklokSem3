// /src/features/veiling/ActueelBod.jsx
import { useContext, useEffect, useMemo, useRef, useState } from "react";
import { Link } from "react-router-dom";
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

    // ✅ Smooth prijs (client-side)
    const [displayPrice, setDisplayPrice] = useState(0);
    const lastServerRef = useRef({
        price: 0,
        at: Date.now(),
        daling: 0,
        min: 0,
    });

    useEffect(() => {
        let alive = true;

        async function load() {
            try {
                const actief = await getPublicActieveVeiling();
                if (!alive) return;

                // jouw public endpoint geeft id=0 terug als geen veiling
                const id = actief?.id && actief.id > 0 ? actief.id : null;

                setVeilingId(id);
                setInitLot(actief?.huidigProduct ?? null);
                setWachtrij(actief?.wachtrij ?? []);
                setError("");
            } catch {
                if (!alive) return;
                setError("Kan actieve veiling niet laden.");
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

    // ✅ Reset displayPrice naar echte server prijs zodra server een update geeft
    useEffect(() => {
        if (!lot) return;

        const serverPrice = Number(lot.huidigePrijs ?? 0);
        const daling = Number(lot.dalingPerSeconde ?? 0); // prijs per seconde
        const min = Number(lot.minimumPrijs ?? 0);

        lastServerRef.current = { price: serverPrice, at: Date.now(), daling, min };
        setDisplayPrice(serverPrice);
    }, [lot?.veilingProductId, lot?.huidigePrijs, lot?.dalingPerSeconde, lot?.minimumPrijs]);

    // ✅ Smooth daling: loopt 4x per sec, maar gebruikt dalingPerSeconde
    useEffect(() => {
        if (!lot) return;

        const t = setInterval(() => {
            const { price, at, daling, min } = lastServerRef.current;
            const elapsedSec = (Date.now() - at) / 1000;

            const smooth = price - daling * elapsedSec;
            const clamped = Math.max(min, smooth); // clamp intern (niet zichtbaar)

            setDisplayPrice(clamped);
        }, 250);

        return () => clearInterval(t);
    }, [lot?.veilingProductId]);

    // ✅ Gebruik displayPrice als "echte prijs" in UI én bij kopen
    const currentPrice = displayPrice ?? 0;

    const maxAantal = useMemo(() => lot?.resterendeHoeveelheid ?? 0, [lot]);

    async function koop() {
        if (!token || role !== "Koper") return;
        if (!veilingId || !lot?.veilingProductId) return;

        const koopAantal = Number(aantal) <= 0 ? 0 : Number(aantal);

        if (koopAantal < 0) {
            alert("Aantal kan niet negatief zijn.");
            return;
        }
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
                    prijs: currentPrice, // ✅ prijs die user ziet
                    aantal: koopAantal,
                }),
            });

            if (!res.ok) {
                let msg = "Kopen mislukt";
                try {
                    const err = await res.json();
                    msg = err?.message || msg;
                } catch { }
                throw new Error(msg);
            }
        } catch (e) {
            alert(e.message);
        }
    }

    if (loadingInit || liveLoading) {
        return (
            <>
                <Navbar />
                <div className="container py-5">
                    <div className="card shadow-sm border-0 p-4">
                        <div className="d-flex align-items-center gap-3">
                            <div className="spinner-border" role="status" aria-hidden="true" />
                            <div>
                                <div className="fw-bold">Veiling laden...</div>
                                <div className="text-muted small">Even geduld, we halen de live status op.</div>
                            </div>
                        </div>
                    </div>
                </div>
                <Footer />
            </>
        );
    }

    if (error) {
        return (
            <>
                <Navbar />
                <div className="container py-5">
                    <div className="card shadow-sm border-0 p-4">
                        <h3 className="fw-bold mb-2">Oeps</h3>
                        <p className="text-muted mb-3">{error}</p>
                        <Link className="btn btn-dark rounded-pill px-4" to="/">
                            Naar homepage
                        </Link>
                    </div>
                </div>
                <Footer />
            </>
        );
    }

    if (!veilingId || !lot) {
        return (
            <>
                <Navbar />
                <div className="container py-5">
                    <div className="card shadow-sm border-0 p-4">
                        <div className="d-flex justify-content-between align-items-start gap-3">
                            <div>
                                <h3 className="fw-bold mb-2">Geen actieve veiling</h3>
                                <p className="text-muted mb-3">
                                    Er is op dit moment geen veiling live. Bekijk de eerstvolgende veiling op de homepage.
                                </p>
                                <Link className="btn btn-dark rounded-pill px-4" to="/">
                                    Naar homepage
                                </Link>
                            </div>
                            <span className="badge bg-secondary align-self-start">OFFLINE</span>
                        </div>
                    </div>
                </div>
                <Footer />
            </>
        );
    }

    const next = wachtrij?.length ? wachtrij[0] : null;

    return (
        <>
            <Navbar />

            <div className="container py-5">
                <div className="d-flex justify-content-between align-items-end flex-wrap gap-3 mb-4">
                    <div>
                        <h2 className="fw-bold mb-1">Actueel bod</h2>
                        <div className="text-muted">Live product + volgende items in wachtrij.</div>
                    </div>
                    <div className="text-muted small">Veiling #{veilingId}</div>
                </div>

                <div className="row g-4">
                    {/* LINKS: huidig product card */}
                    <div className="col-lg-7">
                        <div className="card shadow-sm border-0 overflow-hidden h-100">
                            <div className="row g-0 h-100">
                                <div className="col-md-6">
                                    <img
                                        src={lot.fotoUrl || "/images/flowerbanner1.jpg"}
                                        alt={lot.soort}
                                        className="w-100 h-100 object-fit-cover"
                                        style={{ minHeight: 280 }}
                                    />
                                </div>

                                <div className="col-md-6 p-4">
                                    <div className="d-flex justify-content-between align-items-start">
                                        <div>
                                            <h4 className="fw-bold mb-1">{lot.soort}</h4>
                                            <div className="text-muted small">
                                                Resterend: {lot.resterendeHoeveelheid} stuks
                                            </div>
                                            {!!lot.aanvoerderNaam && (
                                                <div className="text-muted small">Aanvoerder: {lot.aanvoerderNaam}</div>
                                            )}
                                        </div>
                                        <span className="badge bg-success">LIVE</span>
                                    </div>

                                    <div className="mt-4">
                                        <div className="text-muted small">Huidige prijs</div>
                                        <div className="display-6 fw-bold">€{Number(currentPrice).toFixed(2)}</div>

                                        {/*  Minimumprijs niet meer tonen */}

                                        <button
                                            type="button"
                                            className="btn btn-outline-secondary btn-sm mt-3"
                                            onClick={() => setShowHistorie(true)}
                                        >
                                            Prijshistorie bekijken
                                        </button>
                                    </div>

                                    {role === "Koper" ? (
                                        <>
                                            <div className="mt-3">
                                                <label className="form-label mb-1">Aantal (0 = alles)</label>
                                                <input
                                                    type="number"
                                                    className="form-control"
                                                    min="0"
                                                    max={maxAantal}
                                                    value={aantal}
                                                    onChange={(e) => setAantal(e.target.value)}
                                                />
                                                <div className="form-text">Max: {maxAantal}</div>
                                            </div>

                                            <button className="btn btn-dark w-100 rounded-pill mt-3" onClick={koop}>
                                                Koop voor deze prijs
                                            </button>
                                        </>
                                    ) : (
                                        <div className="alert alert-info mt-3 mb-0">
                                            Log in als koper om te kunnen kopen.
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* RECHTS: volgende card */}
                    <div className="col-lg-5">
                        <div className="card shadow-sm border-0 p-4 h-100">
                            <div className="d-flex justify-content-between align-items-center mb-3">
                                <h5 className="fw-bold mb-0">Volgende</h5>
                                <span className="text-muted small">{wachtrij.length} in wachtrij</span>
                            </div>

                            {!next ? (
                                <div className="text-muted">
                                    Geen volgende producten. Dit is het laatste product in deze veiling.
                                </div>
                            ) : (
                                <>
                                    <div className="d-flex gap-3 align-items-center mb-3">
                                        <img
                                            src={next.fotoUrl || "/images/flowerbanner1.jpg"}
                                            alt={next.soort}
                                            style={{ width: 72, height: 72, borderRadius: 14, objectFit: "cover" }}
                                        />
                                        <div>
                                            <div className="fw-semibold">
                                                #{next.volgorde} - {next.soort}
                                            </div>
                                            <div className="text-muted small">
                                                Startprijs: €{Number(next.maximumPrijs ?? 0).toFixed(2)} - Resterend:{" "}
                                                {next.resterendeHoeveelheid}
                                            </div>
                                        </div>
                                    </div>

                                    <div className="list-group list-group-flush">
                                        {wachtrij.slice(1, 4).map((x) => (
                                            <div key={x.veilingProductId} className="list-group-item px-0">
                                                <div className="d-flex justify-content-between">
                                                    <span className="fw-semibold small">{x.soort}</span>
                                                    <span className="text-muted small">#{x.volgorde}</span>
                                                </div>
                                                <div className="text-muted small">
                                                    Start: €{Number(x.maximumPrijs ?? 0).toFixed(2)} - Resterend:{" "}
                                                    {x.resterendeHoeveelheid}
                                                </div>
                                            </div>
                                        ))}
                                    </div>

                                    {wachtrij.length > 4 && (
                                        <div className="text-muted small mt-2">+{wachtrij.length - 4} meer in wachtrij</div>
                                    )}
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </div>

            <PrijsHistorieModal
                open={showHistorie}
                onClose={() => setShowHistorie(false)}
                token={token}
                soort={lot?.soort || ""}
                aanvoerderId={lot?.aanvoerderId}
            />

            <Footer />
        </>
    );
}
