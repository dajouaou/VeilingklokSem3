export default function FeatureHighlights() {
    const items = [
        {
            title: "Realtime bieden zonder verversen",
            text: "Biedupdates zijn direct zichtbaar voor alle deelnemers. Ontworpen voor hoge frequentie en lage latency."
        },
        {
            title: "Aanvoerders & lots overzicht",
            text: "Volgende producten worden gegroepeerd per aanvoerder. Snelle scanning, betere besluitvorming."
        },
        {
            title: "Veilig & compliant by design",
            text: "HTTPS, wachtwoord hashing, AVG-proof dataverwerking en robuuste foutafhandeling als baseline."
        }
    ];

    return (
        <section className="feature-highlights py-5">
            <div className="container">
                <div className="text-center mb-4">
                    <h2 className="fw-bold mb-2"></h2>
                </div>

                <div className="row g-4">
                    {items.map((x, idx) => (
                        <div className="col-md-4" key={idx}>
                            <div className="feature-card h-100">
                                <div className="feature-icon">
                                    <i className="bi bi-lightning-charge"></i>
                                </div>
                                <h5 className="fw-semibold mb-2">{x.title}</h5>
                                <p className="text-muted mb-0">{x.text}</p>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </section>
    );
}