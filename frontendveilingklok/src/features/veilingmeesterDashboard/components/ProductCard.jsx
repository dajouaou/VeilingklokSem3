export default function ProductCard({ lot }) {
    if (!lot) return <div className="card">Geen actief lot</div>;

    return (
        <div className="card product-card">
            <img src={lot.fotoUrl} alt={lot.productNaam} className="product-img" />
            <h3>{lot.productNaam}</h3>
            <p>{lot.categorie}</p>
            <p>{lot.beschrijving}</p>
            <p><strong>Prijs: €{lot.huidigePrijs}</strong></p>
            <p>Bids: {lot.bidCount}</p>
        </div>
    );
}
