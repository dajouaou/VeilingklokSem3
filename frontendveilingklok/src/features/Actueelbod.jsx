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

    const [status, setStatus] = useState(null);
    const isPaused = status === 2; // VeilingStatus.Gepauzeerd

    const [aantal, setAantal] = useState(0);
    const [showHistorie, setShowHistorie] = useState(false);

    // ✅ Smooth prijs
    const [displayPrice, setDisplayPrice] = useState(0);
    const displayPriceRef = useRef(0);
    useEffect(() => {
        displayPriceRef.current = Number(displayPrice ?? 0);
    }, [displayPrice]);

    const lastServerRef = useRef({
        price: 0,
        at: Date.now(),
        daling: 0,
        min: 0,
    });

    const wasPausedRef = useRef(false);

    useEffect(() => {
        let alive = true;

        async function load() {
            try {
                const actief = await getPublicActieveVeiling();
                if (!alive) return;

                const id = actief?.id && actief.id > 0 ? actief.id : null;

                setVeilingId(id);
                setInitLot(actief?.huidigProduct ?? null);
                setWachtrij(actief?.wachtrij ?? []);

                // ✅ Backend stuurt VeilingOverzichtDto: IsPauze/IsGestart/IsAfgesloten (geen status)
                const derivedStatus =
                    actief?.isPauze ? 2 :
                        actief?.isGestart ? 1 :
                            actief?.isAfgesloten ? 3 :
                                null;

                setStatus(derivedStatus);
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

    // ✅ laat live updates altijd lopen; alleen UI-teller pauzeren
    const { lot: liveLot, loading: liveLoading } = useLiveVeiling(token, veilingId);
    const lot = liveLot ?? initLot;

    useEffect(() => {
        setAantal(0);
    }, [lot?.veilingProductId]);

    // ✅ Nieuwe lot: start altijd vanaf serverprijs (bij nieuw product is dat correct)
    useEffect(() => {
        if (!lot) return;

        const serverPrice = Number(lot.huidigePrijs ?? 0);
        const daling = Number(lot.dalingPerSeconde ?? 0);
        const min = Number(lot.minimumPrijs ?? 0);

        lastServerRef.current = { price: serverPrice, at: Date.now(), daling, min };
        setDisplayPrice(serverPrice);
    }, [lot?.veilingProductId]);

    // ✅ Bij pauze: bevries exact op schermprijs en zet daling 0
    useEffect(() => {
        if (!isPaused) return;

        const frozen = Number(displayPriceRef.current ?? 0);
        lastServerRef.current = {
            ...lastServerRef.current,
            price: frozen,
            at: Date.now(),
            daling: 0,
        };
        setDisplayPrice(frozen);
    }, [isPaused]);

    // ✅ Bij resume: GA VERDER VANAF BEVROREN PRIJS (niet van lot.huidigePrijs, die is vaak maximum)
    useEffect(() => {
        if (!lot) return;

        const wasPaused = wasPausedRef.current;
        if (wasPaused && !isPaused) {
            const frozen = Number(displayPriceRef.current ?? 0);
            const daling = Number(lot.dalingPerSeconde ?? 0);
            const min = Number(lot.minimumPrijs ?? 0);

            lastServerRef.current = { price: frozen, at: Date.now(), daling, min };
            setDisplayPrice(frozen);
        }

        wasPausedRef.current = isPaused;
    }, [isPaused, lot?.veilingProductId]);

    // ✅ Smooth daling: alleen lopen als veiling live
    useEffect(() => {
        if (!lot || isPaused) return;

        const t = setInterval(() => {
            const { price, at, daling, min } = lastServerRef.current;
            const elapsedSec = (Date.now() - at) / 1000;

            const smooth = price - daling * elapsedSec;
            const clamped = Math.max(min, smooth);

            setDisplayPrice(clamped);
        }, 250);

        return () => clearInterval(t);
    }, [lot?.veilingProductId, isPaused]);

    const currentPrice = displayPrice ?? 0;
    const maxAantal = useMemo(() => lot?.resterendeHoeveelheid ?? 0, [lot]);

    async function koop() {
        if (!token || role !== "Koper") return;
        if (!veilingId || !lot?.veilingProductId) return;
        if (isPaused) {
            alert("Veiling is momenteel gepauzeerd.");
            return;
        }

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
                    prijs: currentPrice,
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

                {isPaused && (
                    <div className="alert alert-warning mb-3">
                        Veiling is momenteel gepauzeerd
                    </div>
                )}

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
                                        <span className={`badge ${isPaused ? "bg-warning text-dark" : "bg-success"}`}>
                                            {isPaused ? "PAUZE" : "LIVE"}
                                        </span>
                                    </div>

                                    <div className="mt-4">
                                        <div className="text-muted small">Huidige prijs</div>
                                        <div className="display-6 fw-bold">€{Number(currentPrice).toFixed(2)}</div>

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

                                            <button
                                                className="btn btn-dark w-100 rounded-pill mt-3"
                                                onClick={koop}
                                                disabled={isPaused}
                                            >
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
