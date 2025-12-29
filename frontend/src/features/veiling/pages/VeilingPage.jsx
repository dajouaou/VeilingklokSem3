import { useMemo, useState } from "react";
import { useVeilingPublic } from "../hooks/useVeilingPublic";
import "../../../styles/veiling.css";

function Card({ title, right, children }) {
    return (
        <section className="vk-card">
            <div className="vk-card__head">
                <div className="vk-card__title">{title}</div>
                {right ? <div className="vk-card__right">{right}</div> : null}
            </div>
            <div className="vk-card__body">{children}</div>
        </section>
    );
}

function Pill({ children }) {
    return <span className="vk-pill">{children}</span>;
}

function ProductImage({ src, alt }) {
    return (
        <div className="vk-imgwrap">
            {src ? <img className="vk-img" src={src} alt={alt} loading="lazy" /> : <div className="vk-img__empty">Geen foto</div>}
        </div>
    );
}

function formatDateTime(d) {
    if (!d) return "—";
    const dt = new Date(d);
    if (Number.isNaN(dt.getTime())) return "—";
    return new Intl.DateTimeFormat("nl-NL", { dateStyle: "short", timeStyle: "medium" }).format(dt);
}

function money(value) {
    const n = typeof value === "number" ? value : Number(value ?? 0);
    return new Intl.NumberFormat("nl-NL", { style: "currency", currency: "EUR" }).format(Number.isFinite(n) ? n : 0);
}

function getToken() {
    return localStorage.getItem("token") || "";
}

function decodePayload(token) {
    try {
        const part = String(token || "").split(".")[1];
        if (!part) return null;
        const base64 = part.replace(/-/g, "+").replace(/_/g, "/");
        const padded = base64 + "=".repeat((4 - (base64.length % 4)) % 4);
        return JSON.parse(atob(padded));
    } catch {
        return null;
    }
}

function getRoleFromToken(token) {
    const payload = decodePayload(token);
    if (!payload) return "";
    const c =
        payload.role ||
        payload.Role ||
        payload.rol ||
        payload.Rol ||
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"];
    if (typeof c === "string") return c.trim();
    if (Array.isArray(c) && typeof c[0] === "string") return c[0].trim();
    return "";
}

export default function VeilingPage() {
    const veilingId = 1;

    const token = getToken();
    const role = String(getRoleFromToken(token)).toLowerCase();
    const isKoper = role === "koper" || role.includes("koper");

    const [customPrice, setCustomPrice] = useState("");

    const vm = useVeilingPublic({ veilingId, isKoper });
    const current = vm.currentProduct;

    const bidDisabledReason = useMemo(() => {
        if (!isKoper) return "Log in als koper om te bieden";
        if (!current) return "Geen actief product";
        if (!current?.isActief) return "Product is niet actief";
        if (vm.timeLeftSec <= 0) return "Veiling is niet actief";
        if (vm.placing) return "Bezig...";
        return "";
    }, [isKoper, current, vm.timeLeftSec, vm.placing]);

    const onBidNow = async () => {
        await vm.actions.placeBid(null);
    };

    const onBidWithPrice = async () => {
        const raw = (customPrice || "").replace(",", ".");
        const v = raw.trim() === "" ? null : Number(raw);
        if (v == null || Number.isNaN(v)) return;
        await vm.actions.placeBid(v);
    };

    return (
        <div className="vk-page">
            <header className="vk-header">
                <div className="vk-header__left">
                    <div className="vk-h1">Veiling</div>
                    <div className="vk-sub">Realtime kopen op actuele prijs</div>
                </div>
                <div className="vk-header__right">
                    <Pill>Veiling #{veilingId}</Pill>
                    <Pill>Prijs: {vm.ui.priceText}</Pill>
                    <Pill>Tijd: {vm.ui.timeText}</Pill>
                </div>
            </header>

            {vm.error ? <div className="vk-alert vk-alert--error">{vm.error}</div> : null}
            {vm.toast ? <div className="vk-alert vk-alert--ok">{vm.toast}</div> : null}

            <div className="vk-grid">
                <div className="vk-col">
                    <Card title="Actueel product" right={current ? <Pill>{current.isActief ? "Actief" : "Niet actief"}</Pill> : <Pill>Geen actief product</Pill>}>
                        {!current ? (
                            <div className="vk-empty">
                                <div className="vk-empty__title">Geen actief product op dit moment</div>
                                <div className="vk-empty__text">Bekijk de wachtrij hieronder.</div>
                            </div>
                        ) : (
                            <div className="vk-product">
                                <ProductImage src={current.fotoUrl || null} alt={current.soort || "Product"} />
                                <div className="vk-product__info">
                                    <div className="vk-product__name">{current.soort}</div>
                                    <div className="vk-kv">
                                        <div className="vk-kv__row">
                                            <span>Hoeveelheid</span>
                                            <span>{current.hoeveelheid}</span>
                                        </div>
                                        <div className="vk-kv__row">
                                            <span>Startprijs</span>
                                            <span>{money(current.startPrijs)}</span>
                                        </div>
                                        <div className="vk-kv__row">
                                            <span>Huidige prijs</span>
                                            <span className="vk-strong">{money(vm.currentPrice)}</span>
                                        </div>
                                        <div className="vk-kv__row">
                                            <span>Resterende tijd</span>
                                            <span className="vk-strong">{vm.ui.timeText}</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        )}
                    </Card>

                    <Card title="Bieden" right={!isKoper ? <Pill>Read-only</Pill> : <Pill>Koper</Pill>}>
                        <div className="vk-bid">
                            <button className="vk-btn vk-btn--primary" disabled={!vm.canBid} onClick={onBidNow}>
                                Bied nu (koop op huidige prijs)
                            </button>

                            <div className="vk-bid__hint">{vm.canBid ? "Een bod verkoopt direct en het volgende product wordt automatisch geactiveerd." : bidDisabledReason}</div>

                            <div className="vk-bid__row">
                                <input
                                    className="vk-input"
                                    value={customPrice}
                                    onChange={(e) => setCustomPrice(e.target.value)}
                                    placeholder="Optioneel: prijs (decimal)"
                                    inputMode="decimal"
                                />
                                <button className="vk-btn" disabled={!vm.canBid || customPrice.trim() === ""} onClick={onBidWithPrice}>
                                    Verstuur prijs
                                </button>
                            </div>
                        </div>
                    </Card>
                </div>

                <div className="vk-col">
                    <Card title="Biedingen" right={<Pill>{vm.bids.length} recent</Pill>}>
                        {vm.loading ? (
                            <div className="vk-skeleton">Laden...</div>
                        ) : vm.bids.length === 0 ? (
                            <div className="vk-empty">
                                <div className="vk-empty__title">Nog geen biedingen</div>
                                <div className="vk-empty__text">Zodra kopers bieden, zie je het hier.</div>
                            </div>
                        ) : (
                            <div className="vk-list">
                                {vm.bids.slice(0, 20).map((b, idx) => {
                                    const id = b.id ?? b.bidId ?? `${idx}`;
                                    const koperNaam = b.koperNaam ?? b.koper ?? "Koper";
                                    const tijd = b.placedAtUtc ?? b.tijdstip ?? b.time ?? null;
                                    const bedrag = b.amount ?? b.prijs ?? b.price ?? 0;

                                    return (
                                        <div className="vk-list__item" key={id}>
                                            <div className="vk-list__left">
                                                <div className="vk-list__title">{koperNaam}</div>
                                                <div className="vk-list__sub">{formatDateTime(tijd)}</div>
                                            </div>
                                            <div className="vk-list__right">{money(bedrag)}</div>
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </Card>

                    <Card title="Wachtrij" right={<Pill>{vm.queue.length} items</Pill>}>
                        {vm.queue.length === 0 ? (
                            <div className="vk-empty">
                                <div className="vk-empty__title">Geen wachtrij</div>
                                <div className="vk-empty__text">Er zijn geen volgende producten.</div>
                            </div>
                        ) : (
                            <div className="vk-queue">
                                {vm.queue.map((p) => (
                                    <div className="vk-queueitem" key={p.veilingProductId ?? p.id}>
                                        <div className="vk-queueitem__thumb">
                                            <ProductImage src={p.fotoUrl || null} alt={p.soort || "Product"} />
                                        </div>
                                        <div className="vk-queueitem__meta">
                                            <div className="vk-queueitem__title">{p.soort}</div>
                                            <div className="vk-queueitem__sub">
                                                <span>Hoeveelheid {p.hoeveelheid}</span>
                                                <span>•</span>
                                                <span>Start {money(p.startPrijs)}</span>
                                            </div>
                                        </div>
                                        <div className="vk-queueitem__right">{money(p.startPrijs)}</div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </Card>
                </div>
            </div>
        </div>
    );
}
