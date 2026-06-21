import { createServer } from "node:http";
import { spawn } from "node:child_process";
import { writeFileSync } from "node:fs";

const port = Number(process.env.PORT || 43124);
const child = spawn(process.execPath, ["child.mjs"], {
  stdio: "inherit",
  cwd: process.cwd()
});

writeFileSync("server.pid", String(process.pid));
writeFileSync("child.pid", String(child.pid));

const server = createServer((req, res) => {
  res.writeHead(200, { "content-type": "text/plain" });
  res.end(`workbox fixture pid=${process.pid} child=${child.pid} url=${req.url}`);
});

server.listen(port, "127.0.0.1", () => {
  console.log(`fixture ready http://127.0.0.1:${port}`);
});

process.on("SIGTERM", () => {
  child.kill();
  server.close(() => process.exit(0));
});
