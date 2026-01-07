import { useContext, useEffect, useMemo, useState } from "react";
import Navbar from "../shared/components/Navbar";
import Footer from "../shared/components/Footer";
import { AuthContext } from "./auth/AuthContext";
import useLiveVeiling from "./veilingmeesterDashboard/hooks/useLiveVeiling";
import { getPublicActieveVeiling } from "./veiling/api/veilingPublicApi";

const API_BASE = "https://localhost:56418";

export default function ActueelBod() {
    const { token, role } = useContext(AuthContext);

    const [veilingId, setVeilingId] = useState(null);
    const [initLot, setInitLot] = useState(null); // ✅ fallback als SignalR nog niets stuurde
    const [loadingInit, setLoadingInit] = useState(true);
    const [error, setError] = useState("");

    const [aantal, setAantal] = useState(0);

    // 1) haal actuele veiling + huidig product op (public)
    useEffect(() => {
        async function load() {
            setError("");
            setLoadingInit(true);

            try {
                const actief = await getPublicActieveVeiling();
                setVeilingId(actief?.id ?? null);
                setInitLot(actief?.huidigProduct ?? null);
            } catch (e) {
                console.error(e);
                setError("Kon actieve veiling niet ophalen.");
                setVeilingId(null);
                setInitLot(null);
            } finally {
                setLoadingInit(false);
            }
        }

        load();
    }, []);

    // 2) live data via SignalR (jouw hook)
    const { lot: liveLot, loading: liveLoading } = useLiveVeiling(token, veilingId);

    // ✅ kies liveLot als die er is, anders initLot
    const lot = liveLot ?? initLot;

    // reset aantal bij nieuw product
    useEffect(() => {
        setAantal(0);
    }, [lot?.veilingProductId]);

    const currentPrice = lot?.huidigePrijs ?? 0;

    const maxAantal = useMemo(() => lot?.resterendeHoeveelheid ?? 0, [lot]);

    async function koop() {
        if (!token || role !== "Koper") return;
        if (!veilingId || !lot?.veilingProductId) return;

        // 0 = alles (jouw backend conventie)
        const koopAantal = Number(aantal) <= 0 ? 0 : Number(aantal);

        // simpele client-side guard
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
                    prijs: currentPrice,  // prijs vastleggen op dit moment
                    aantal: koopAantal,   // 0 => alles
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

            // ✅ na succesvolle koop: refresh init state (handig als je net in pauze/zonder events zat)
            try {
                const actief = await getPublicActieveVeiling();
                setInitLot(actief?.huidigProduct ?? null);
            } catch { }
        } catch (e) {
            alert(e.message);
        }
    }

    // UI states
    if (loadingInit || liveLoading) {
        return (
            <>
                <Navbar />
                <div className="container py-5">Veiling laden…</div>
                <Footer />
            </>
        );
    }

    if (error) {
        return (
            <>
                <Navbar />
                <div className="container py-5 text-danger">{error}</div>
                <Footer />
            </>
        );
    }

    if (!veilingId) {
        return (
            <>
                <Navbar />
                <div className="container py-5">Geen actieve veiling</div>
                <Footer />
            </>
        );
    }

    // ✅ veiling bestaat maar huidig product ontbreekt (bv. net afgesloten / net doordraai overgang)
    if (!lot) {
        return (
            <>
                <Navbar />
                <div className="container py-5">
                    Veiling is actief, maar er is momenteel geen huidig product.
                </div>
                <Footer />
            </>
        );
    }

    return (
        <>
            <Navbar />
            <div className="container py-5">
                <h2 className="fw-bold mb-4">Actueel bod</h2>

                <div className="row">
                    <div className="col-md-6">
                        <img
                            src={lot.fotoUrl || "/images/bloemen.jpg"}
                            alt={lot.soort}
                            className="img-fluid rounded"
                        />
                    </div>

                    <div className="col-md-6">
                        <h4 className="mt-2">{lot.soort}</h4>
                        <small>Resterend: {lot.resterendeHoeveelheid} stuks</small>

                        <h5 className="mt-4">Huidige prijs: € {currentPrice.toFixed(2)}</h5>

                        {role === "Koper" && (
                            <>
                                <div className="mt-3">
                                    <label className="form-label">Aantal (0 = alles)</label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        min="0"
                                        max={maxAantal}
                                        value={aantal}
                                        onChange={(e) => setAantal(e.target.value)}
                                    />
                                    <div className="form-text">
                                        Max: {maxAantal}. Laat op 0 staan om alles te kopen.
                                    </div>
                                </div>

                                <button className="btn btn-dark mt-3" onClick={koop}>
                                    Koop voor deze prijs
                                </button>
                            </>
                        )}
                    </div>
                </div>
            </div>
            <Footer />
        </>
    );
}
