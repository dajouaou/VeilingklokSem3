import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getPublicActieveVeiling, getPublicVolgendeVeiling } from "../../features/veiling/api/veilingPublicApi";
import flowerbanner1 from "../../Images/flowerbanner1.jpg";

export default function HeroBanner({ isLoggedIn, isKoper }) {
    // State voor hero data
    const [data, setData] = useState({
        titel: "Geen actieve veiling",
        beschrijving: "Er is momenteel geen veiling actief.",
        bid: "-",
        afbeelding: flowerbanner1,
    });

    const [volgendeVeiling, setVolgendeVeiling] = useState(null);

    // Countdown helper
    function getCountdown(startDatum, startTijd) {
        if (!startDatum || !startTijd) return "";
        const target = new Date(`${startDatum}T${startTijd}`);
        const diff = target - new Date();
        if (diff <= 0) return "Start elk moment";

        const totalSec = Math.floor(diff / 1000);
        const min = Math.floor(totalSec / 60);
        const sec = totalSec % 60;
        return `${min}m ${String(sec).padStart(2, "0")}s`;
    }

    // Laad actieve of volgende veiling
    useEffect(() => {
        let alive = true;

        async function load() {
            try {
                const actief = await getPublicActieveVeiling();
                if (!alive) return;

                // Actieve veiling
                if (actief?.huidigProduct) {
                    const p = actief.huidigProduct;
                    setVolgendeVeiling(null);
                    setData({
                        titel: p.soort,
                        beschrijving: `Resterend: ${p.resterendeHoeveelheid} stuks`,
                        bid: `${Number(p.huidigePrijs ?? 0).toFixed(2)} EUR`,
                        afbeelding: p.fotoUrl || flowerbanner1,
                    });
                    return;
                }

                // Volgende veiling
                const volgende = await getPublicVolgendeVeiling();
                if (!alive) return;

                if (volgende) {
                    setVolgendeVeiling(volgende);
                    setData({
                        titel: `Volgende veiling #${volgende.id}`,
                        beschrijving: `Start over ${getCountdown(volgende.veildatum, volgende.startTijd)} • ${volgende.aantalProducten} producten`,
                        bid: "-",
                        afbeelding: flowerbanner1,
                    });
                    return;
                }

                // Geen veiling
                setVolgendeVeiling(null);
                setData({
                    titel: "Geen actieve veiling",
                    beschrijving: "Er is momenteel geen veiling actief.",
                    bid: "-",
                    afbeelding: flowerbanner1,
                });
            } catch {
                // Fallback bij error
                if (!alive) return;
            }
        }

        load();
        const interval = setInterval(load, 5000);

        return () => {
            alive = false;
            clearInterval(interval);
        };
    }, []);

    // Update countdown elke seconde
    useEffect(() => {
        if (!volgendeVeiling) return;

        const interval = setInterval(() => {
            setData((prev) => ({
                ...prev,
                beschrijving: `Start over ${getCountdown(
                    volgendeVeiling.veildatum,
                    volgendeVeiling.startTijd
                )} • ${volgendeVeiling.aantalProducten} producten`,
            }));
        }, 1000);

        return () => clearInterval(interval);
    }, [volgendeVeiling]);

    // Status label
    const status = data.titel.startsWith("Volgende veiling")
        ? "Gepland"
        : data.titel === "Geen actieve veiling"
            ? "Geen"
            : "Live";

    // Knop gedrag
    const buttonLink = isKoper ? "/actueelbod" : "/login";
    const buttonText = isKoper ? "Ga naar Actueel Bod" : "Log in om mee te doen";

    return (
        <section className="hero-section py-5">
            <div className="container-fluid px-0">
                <div className="row g-0 align-items-center">
                    <div className="col-md-6 p-5 hero-text">
                        <span className="info-chip mb-3 d-inline-block">
                            Status: {status}
                        </span>

                        <h1 className="fw-bold mb-3">{data.titel}</h1>
                        <p className="text-muted mb-4">{data.beschrijving}</p>

                        {/* Prijs alleen tonen voor ingelogde kopers */}
                        {isKoper && (
                            <div className="bid-info mb-4">
                                <div className="last-bid-box">
                                    <span className="label">Huidige prijs</span>
                                    <span className="value">{data.bid}</span>
                                </div>
                            </div>
                        )}

                        <Link to={buttonLink} className="btn" id="btn-herobanner">
                            {buttonText}
                        </Link>
                    </div>

                    <div className="col-md-6 hero-image">
                        <img
                            src={data.afbeelding}
                            className="hero-banner-image"
                            alt="Veiling banner"
                        />
                    </div>
                </div>
            </div>
        </section>
    );
}
