// Development service worker: network-only (no offline cache).
// Avoid a no-op fetch listener — browsers warn that it adds navigation overhead.
self.addEventListener('install', event => self.skipWaiting());
self.addEventListener('activate', event => event.waitUntil(self.clients.claim()));
