import { useEffect, useState } from "react";
import { fetchPrijsHistorie } from "/src/features/veiling/api/prijsHistorieApi";

export default function PrijsHistorieModal({
    open,
    onClose,
    token,
    soort,
    aanvoerderId
}) {
    const [loading, setLoading] = useState(false);
    const [data, setData] = useState(null);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!open) return;

        let alive = true;
        setLoading(true);
        setError("");
        setData(null);

        fetchPrijsHistorie({ token, soort, aanvoerderId })
            .then((d) => {
                if (!alive) return;
                setData(d);
            })
            .catch((e) => {
                if (!alive) return;
                setError(e?.message || "Kon prijshistorie niet laden.");
            })
            .finally(() => {
                if (!alive) return;
                setLoading(false);
            });

        return () => {
            alive = false;
        };
    }, [open, token, soort, aanvoerderId]);

    if (!open) return null;

    return (
        <div
            className="modal d-block"
            tabIndex="-1"
            role="dialog"
            style={{ background: "rgba(0,0,0,.45)" }}
            onClick={onClose}
        >
            <div className="modal-dialog modal-lg" role="document" onClick={(e) => e.stopPropagation()}>
                <div className="modal-content">

                    <div className="modal-header">
                        <h5 className="modal-title">Prijshistorie – {soort || "-"}</h5>
                        <button className="btn-close" onClick={onClose} aria-label="Sluiten" />
                    </div>

                    <div className="modal-body">
                        {loading && <div>Laden…</div>}
                        {error && <div className="alert alert-danger">{error}</div>}

                        {!loading && !error && data && (
                            <div className="row g-3">
                                {/* Alle aanvoerders */}
                                <div className="col-md-6">
                                    <div className="card p-3">
                                        <h6 className="mb-2">Alle aanvoerders</h6>

                                        <div className="mb-2">
                                            <b>Gemiddelde:</b>{" "}
                                            {data.gemiddeldeAlleAanvoerders != null
                                                ? `${Number(data.gemiddeldeAlleAanvoerders).toFixed(2)} EUR`
                                                : "-"}
                                        </div>

                                        <div className="table-responsive">
                                            <table className="table table-sm mb-0">
                                                <thead>
                                                    <tr>
                                                        <th>Prijs</th>
                                                        <th>Tijd</th>
                                                        <th>Aanvoerder</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    {data.laatste10AlleAanvoerders?.length ? (
                                                        data.laatste10AlleAanvoerders.map((p, i) => (
                                                            <tr key={i}>
                                                                <td>{Number(p.prijs).toFixed(2)} EUR</td>
                                                                <td>{p.tijdstip ? new Date(p.tijdstip).toLocaleString("nl-NL") : "-"}</td>
                                                                <td>{p.aanvoerderNaam ?? "-"}</td>
                                                            </tr>
                                                        ))
                                                    ) : (
                                                        <tr>
                                                            <td colSpan="3" className="text-muted">Geen data</td>
                                                        </tr>
                                                    )}
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>

                                {/* Huidige aanvoerder */}
                                <div className="col-md-6">
                                    <div className="card p-3">
                                        <h6 className="mb-2">Huidige aanvoerder</h6>

                                        {!aanvoerderId && (
                                            <div className="alert alert-warning py-2">
                                                AanvoerderId ontbreekt in “lot”. Voeg dit toe in de backend DTO om dit blok te vullen.
                                            </div>
                                        )}

                                        <div className="mb-2">
                                            <b>Gemiddelde:</b>{" "}
                                            {data.gemiddeldeHuidigeAanvoerder != null
                                                ? `${Number(data.gemiddeldeHuidigeAanvoerder).toFixed(2)} EUR`
                                                : "-"}
                                        </div>

                                        <div className="table-responsive">
                                            <table className="table table-sm mb-0">
                                                <thead>
                                                    <tr>
                                                        <th>Prijs</th>
                                                        <th>Tijd</th>
                                                        <th>Aanvoerder</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    {data.laatste10HuidigeAanvoerder?.length ? (
                                                        data.laatste10HuidigeAanvoerder.map((p, i) => (
                                                            <tr key={i}>
                                                                <td>{Number(p.prijs).toFixed(2)} EUR</td>
                                                                <td>{p.tijdstip ? new Date(p.tijdstip).toLocaleString("nl-NL") : "-"}</td>
                                                                <td>{p.aanvoerderNaam ?? "-"}</td>
                                                            </tr>
                                                        ))
                                                    ) : (
                                                        <tr>
                                                            <td colSpan="3" className="text-muted">Geen data</td>
                                                        </tr>
                                                    )}
                                                </tbody>
                                            </table>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        )}

                        {!loading && !error && !data && (
                            <div className="text-muted">Geen data ontvangen.</div>
                        )}
                    </div>

                    <div className="modal-footer">
                        <button className="btn btn-secondary" onClick={onClose}>Sluiten</button>
                    </div>

                </div>
            </div>
        </div>
    );
}
