import { useEffect, useState } from "react";
import { getHome } from "../api/homeApi";

export function useHome() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        let alive = true;

        (async () => {
            try {
                setLoading(true);
                setError("");
                const dto = await getHome();
                if (alive) setData(dto);
            } catch (e) {
                if (alive) setError(String(e?.message ?? e));
            } finally {
                if (alive) setLoading(false);
            }
        })();

        return () => {
            alive = false;
        };
    }, []);

    return { data, loading, error };
}

