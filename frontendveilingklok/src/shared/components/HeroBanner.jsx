import { useEffect, useState } from "react";
import { getPublicActieveVeiling, getPublicVolgendeVeiling } from "../../features/veiling/api/veilingPublicApi";
import { Link } from "react-router-dom";

export default function HeroBanner() {
    const [data, setData] = useState({
        titel: "Geen actieve veiling",
        beschrijving: "Er is momenteel geen veiling actief.",
        bid: "-",
        afbeelding: "/images/bloemen.jpg",
        link: "/actueelbod",
    });

    const [volgendeVeiling, setVolgendeVeiling] = useState(null);

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

    useEffect(() => {
        let alive = true;

        async function load() {
            try {
                const actief = await getPublicActieveVeiling();
                if (!alive) return;

                if (actief?.huidigProduct) {
                    const p = actief.huidigProduct;
                    setVolgendeVeiling(null);

                    setData({
                        titel: p.soort,
                        beschrijving: `Resterend: ${p.resterendeHoeveelheid} stuks`,
                        bid: `${(p.huidigePrijs ?? 0).toFixed(2)} EUR`,
                        afbeelding: p.fotoUrl || "/images/bloemen.jpg",
                        link: "/actueelbod",
                    });
                    return;
                }

                const volgende = await getPublicVolgendeVeiling();
                if (!alive) return;

                if (volgende) {
                    setVolgendeVeiling(volgende);
                    const tekst = getCountdown(volgende.veildatum, volgende.startTijd);

                    setData({
                        titel: `Volgende veiling #${volgende.id}`,
                        beschrijving: `Start over ${tekst} • ${volgende.aantalProducten} producten`,
                        bid: "-",
                        afbeelding: "/images/bloemen.jpg",
                        link: "/actueelbod",
                    });
                    return;
                }

                setVolgendeVeiling(null);
                setData({
                    titel: "Geen actieve veiling",
                    beschrijving: "Er is momenteel geen veiling actief.",
                    bid: "-",
                    afbeelding: "/images/bloemen.jpg",
                    link: "/actueelbod",
                });
            } catch (err) {
                console.error("Kan banner niet laden:", err);
            }
        }

        load();
        const interval = setInterval(load, 5000);

        return () => {
            alive = false;
            clearInterval(interval);
        };
    }, []);

    // countdown live update elke seconde
    useEffect(() => {
        if (!volgendeVeiling) return;

        const interval = setInterval(() => {
            const tekst = getCountdown(volgendeVeiling.veildatum, volgendeVeiling.startTijd);
            setData((prev) => ({
                ...prev,
                beschrijving: `Start over ${tekst} • ${volgendeVeiling.aantalProducten} producten`,
            }));
        }, 1000);

        return () => clearInterval(interval);
    }, [volgendeVeiling]);

    const status = data.titel.startsWith("Volgende veiling") ? "Gepland" : data.titel === "Geen actieve veiling" ? "Geen" : "Live";

    return (
        <section className="hero-section py-5">
            <div className="container-fluid px-0">
                <div className="row g-0 align-items-center">
                    <div className="col-md-6 p-5 hero-text">
                        <div className="mb-3 d-flex gap-2 flex-wrap">
                            <span className="info-chip">
                                <span className="label">Status</span>
                                <span className="value">{status}</span>
                            </span>
                        </div>

                        <h1 className="fw-bold mb-3">{data.titel}</h1>
                        <p className="text-muted mb-4">{data.beschrijving}</p>

                        <div className="bid-info d-flex align-items-center gap-3 mb-4">
                            <div className="last-bid-box">
                                <span className="label">Huidige prijs</span>
                                <span className="value">{data.bid}</span>
                            </div>
                        </div>

                        <Link to={data.link} className="btn" id="btn-herobanner">
                            Bekijk veiling
                        </Link>
                    </div>

                    <div className="col-md-6 hero-image">
                        <img
                            src={data.afbeelding}
                            className="img-fluid w-100 h-100 object-fit-cover"
                            alt="Veiling banner"
                        />
                    </div>
                </div>
            </div>
        </section>
    );
}
