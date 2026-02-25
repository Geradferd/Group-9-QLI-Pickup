import { useEffect, useState } from "react";

type PingResponse = { message: string };

export default function App() {
  const [msg, setMsg] = useState<string>("Loading...");

  useEffect(() => {
    fetch("/api/ping")
      .then((r) => {
        if (!r.ok) throw new Error(`HTTP ${r.status}`);
        return r.json() as Promise<PingResponse>;
      })
      .then((d) => setMsg(d.message))
      .catch(() => setMsg("error"));
  }, []);

  return <h1>{msg}</h1>;
}