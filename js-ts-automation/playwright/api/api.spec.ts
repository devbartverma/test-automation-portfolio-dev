import { test, expect } from '@playwright/test';

/**
 * API contract tests against JSONPlaceholder — a free, stable, public REST API.
 * No browser is launched (see the `api` project in playwright.config.ts).
 */
const BASE = 'https://jsonplaceholder.typicode.com';

test.describe('JSONPlaceholder REST API', () => {
  // Data-driven: the same contract is asserted for several resources.
  for (const postId of [1, 2, 3]) {
    test(`GET /posts/${postId} returns a post matching the schema`, async ({ request }) => {
      const response = await request.get(`${BASE}/posts/${postId}`);
      expect(response.status()).toBe(200);

      const body = await response.json();
      expect(body.id).toBe(postId);
      expect(typeof body.userId).toBe('number');
      expect(body.title.length).toBeGreaterThan(0);
      expect(body.body.length).toBeGreaterThan(0);
    });
  }

  test('GET /posts returns the full collection of 100 posts', async ({ request }) => {
    const response = await request.get(`${BASE}/posts`);
    expect(response.status()).toBe(200);
    expect(await response.json()).toHaveLength(100);
  });

  test('POST /posts creates a resource and echoes the payload', async ({ request }) => {
    const payload = { title: 'SDET portfolio', body: 'created via API test', userId: 1 };
    const response = await request.post(`${BASE}/posts`, { data: payload });
    expect(response.status()).toBe(201);

    const body = await response.json();
    expect(body.title).toBe(payload.title);
    expect(body.body).toBe(payload.body);
    expect(typeof body.id).toBe('number');
  });
});
