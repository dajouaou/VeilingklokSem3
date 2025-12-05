import { useEffect, useState } from "react";
import { createAuctionHubConnection } from "../../signalr/auctionHubConnection";

export function useAuctionHub({ token, veilingId }) {
    const [connection, setConnection] = useState(null);
    const [currentLot, setCurrentLot] = useState(null);
    const [currentPrice, setCurrentPrice] = useState(null);
    const [bids, setBids] = useState([]);
    const [isStopped, setIsStopped] = useState(false);

    useEffect(() => {
        if (!token || !veilingId) return;

        const conn = createAuctionHubConnection(token);
        setConnection(conn);

        async function start() {
            try {
                await conn.start();
                console.log("SignalR connected");

                // vertellen aan hub dat we in deze veiling willen zitten
                await conn.invoke("JoinVeilingGroup", veilingId);

                // event handlers
                conn.on("CurrentLotUpdated", (lotDto) => {
                    setCurrentLot(lotDto);
                    setCurrentPrice(lotDto.huidigePrijs ?? lotDto.startPrijs ?? null);
                    setBids([]);
                    setIsStopped(false);
                });

                conn.on("PriceUpdated", (price) => {
                    setCurrentPrice(price);
                });

                conn.on("BidPlaced", (bidDto) => {
                    setBids((prev) => [bidDto, ...prev]);
                });

                conn.on("AuctionStopped", () => {
                    setIsStopped(true);
                });
            } catch (err) {
                console.error("SignalR connect error:", err);
            }
        }

        start();

        // cleanup
        return () => {
            (async () => {
                try {
                    await conn.invoke("LeaveVeilingGroup", veilingId);
                } catch (_) { }

                conn.stop().catch(() => { });
            })();
        };
    }, [token, veilingId]);

    return {
        connection,
        currentLot,
        currentPrice,
        bids,
        isStopped,
    };
}
