import { useEffect, useMemo, useState } from "react";

function groupByAanvoerder(queue) {
    const map = new Map();
    queue.forEach((q) => {
        const key = q.aanvoerderNaam || "Onbekende aanvoerder";
        if (!map.has(key)) map.set(key, []);
        map.get(key).push(q);
    });
    return Array.from(map.entries()).map(([name, items]) => ({
        name,
        items: items.slice().sort((a, b) => a.volgorde - b.volgorde || a.id - b.id),
    }));
}

export function QueuePanel({ status, queue, actions }) {
    const [localIds, setLocalIds] = useState([]);

    const canManage =
        String(status) !== "Running" && String(status) !== "Paused" && String(status) !== "Finished";

    const orderedQueue = useMemo(() => {
        const list = Array.isArray(queue) ? queue : [];
        if (!localIds.length) return list;
        const byId = new Map(list.map((x) => [x.id, x]));
        const rebuilt = localIds.map((id) => byId.get(id)).filter(Boolean);
        const missing = list.filter((x) => !localIds.includes(x.id));
        return [...rebuilt, ...missing];
    }, [queue, localIds]);

    const grouped = useMemo(() => groupByAanvoerder(orderedQueue), [orderedQueue]);

    useEffect(() => {
        const list = Array.isArray(queue) ? queue : [];
        setLocalIds(list.map((x) => x.id));
    }, [queue]);

    function move(id, dir) {
        setLocalIds((prev) => {
            const i = prev.indexOf(id);
            if (i < 0) return prev;
            const j = i + dir;
            if (j < 0 || j >= prev.length) return prev;
            const next = prev.slice();
            const tmp = next[i];
            next[i] = next[j];
            next[j] = tmp;
            return next;
        });
    }

    async function save() {
        await actions.reorder(localIds);
    }

    return (
        <div className="vm-panel">
            <div className="vm-panel__header">
                <div className="vm-panel__title">Wachtrij</div>
                <div className="vm-panel__hint">{Array.isArray(queue) ? queue.length : 0} items</div>
            </div>

            {!Array.isArray(queue) || queue.length === 0 ? (
                <div className="vm-empty">
                    <div className="vm-empty__title">Wachtrij is leeg</div>
                    <div className="vm-empty__text">Koppel producten aan deze veiling.</div>
                </div>
            ) : (
                <div className="vm-queue">
                    {grouped.map((g) => (
                        <div key={g.name} className="vm-queue__group">
                            <div className="vm-queue__groupTitle">{g.name}</div>
                            <div className="vm-queue__items">
                                {g.items.map((q) => (
                                    <div key={q.id} className="vm-queue__item">
                                        <div className="vm-queue__left">
                                            <div className="vm-queue__thumb">
                                                {q.fotoUrl ? <img src={q.fotoUrl} alt={q.productNaam} /> : <div className="vm-thumb--placeholder">—</div>}
                                            </div>
                                            <div className="vm-queue__text">
                                                <div className="vm-queue__name">{q.productNaam}</div>
                                                <div className="vm-queue__meta">
                                                    #{q.volgorde} • {q.hoeveelheid} • {String(q.status)}
                                                </div>
                                            </div>
                                        </div>

                                        <div className="vm-queue__actions">
                                            <button className="vm-btn vm-btn--small" disabled={!canManage} onClick={() => move(q.id, -1)} type="button">
                                                Omhoog
                                            </button>
                                            <button className="vm-btn vm-btn--small" disabled={!canManage} onClick={() => move(q.id, 1)} type="button">
                                                Omlaag
                                            </button>
                                            <button
                                                className="vm-btn vm-btn--small"
                                                disabled={String(status) === "Finished"}
                                                onClick={() => actions.skip(q.id)}
                                                type="button"
                                                title="Zet dit product achteraan"
                                            >
                                                Overslaan
                                            </button>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    ))}

                    <div className="vm-queue__footer">
                        <button className="vm-btn" disabled={!canManage} onClick={save} type="button">
                            Volgorde opslaan
                        </button>
                        {!canManage && <div className="vm-note">Wachtrij aanpassen kan niet tijdens Running/Paused/Finished.</div>}
                    </div>
                </div>
            )}
        </div>
    );
}
