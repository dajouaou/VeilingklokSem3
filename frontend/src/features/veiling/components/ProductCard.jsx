// src/features/veiling/components/ProductCard.jsx
export default function ProductCard({ product, currentPrice, isHighestBidder }) {
    if (!product) {
        return <p>Er is momenteel geen actief product.</p>;
    }

    const { product: prodInfo, aanvoerder, hoeveelheid, status, startPrijs, huidigePrijs } =
        product;

    const naam = prodInfo?.naam ?? "Onbekend product";
    const fotoUrl = prodInfo?.fotoUrl;
    const soort = prodInfo?.soort;
    const categorie = prodInfo?.categorie;
    const potmaat = prodInfo?.potmaatOfSteellengte;
    const aanvoerderNaam = aanvoerder?.naam ?? "Onbekende aanvoerder";

    const prijsOmTeTonen = typeof currentPrice === "number" ? currentPrice : huidigePrijs;

    return (
        <div
            style={{
                border: "1px solid #ddd",
                borderRadius: "8px",
                padding: "1rem",
                display: "flex",
                gap: "1rem",
            }}
        >
            {fotoUrl && (
                <img
                    src={fotoUrl}
                    alt={naam}
                    style={{
                        width: "160px",
                        height: "160px",
                        objectFit: "cover",
                        borderRadius: "8px",
                    }}
                />
            )}

            <div style={{ flex: 1 }}>
                <h2 style={{ marginTop: 0 }}>{naam}</h2>
                <p>Status: <strong>{status}</strong></p>
                <p>Hoeveelheid: {hoeveelheid}</p>
                <p>Startprijs: € {startPrijs?.toFixed(2)}</p>
                <p>Huidige prijs: € {prijsOmTeTonen?.toFixed(2)}</p>
                {soort && <p>Soort: {soort}</p>}
                {categorie && <p>Categorie: {categorie}</p>}
                {potmaat && <p>Potmaat / steellengte: {potmaat}</p>}
                <p>Aangevoerd door: {aanvoerderNaam}</p>

                {isHighestBidder && (
                    <p style={{ marginTop: "0.5rem", color: "green" }}>
                        Jij hebt op dit moment het hoogste bod.
                    </p>
                )}
            </div>
        </div>
    );
}
