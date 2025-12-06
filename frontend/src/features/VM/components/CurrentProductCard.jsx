export default function CurrentProductCard({ product }) {
    if (!product) {
        return <div className="card">Geen actief product</div>;
    }

    return (
        <div className="card">
            <h2>Actief product</h2>

            <p><strong>Naam:</strong> {product.productNaam}</p>
            <p><strong>Aanvoerder:</strong> {product.aanvoerder}</p>
            <p><strong>Startprijs:</strong> €{product.startPrijs}</p>
            <p><strong>Huidige prijs:</strong> €{product.huidigePrijs}</p>
            <p><strong>Hoeveelheid:</strong> {product.hoeveelheid} stuks</p>
        </div>
    );
}
