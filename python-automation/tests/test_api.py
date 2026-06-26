import requests
import pytest

# API contract tests against JSONPlaceholder — a free, stable, public REST API.
# These do not use the `page` fixture, so no browser is launched.
BASE = "https://jsonplaceholder.typicode.com"
TIMEOUT = 30


class TestApi:
    """REST API contract / schema tests."""

    @pytest.mark.parametrize("post_id", [1, 2, 3])
    def test_get_post_by_id_matches_schema(self, post_id):
        """
        Given: the JSONPlaceholder API
        When: I GET a single post by id (data-driven over several ids)
        Then: it returns 200 and a body matching the expected schema
        """
        response = requests.get(f"{BASE}/posts/{post_id}", timeout=TIMEOUT)

        assert response.status_code == 200
        body = response.json()
        assert body["id"] == post_id
        assert isinstance(body["userId"], int)
        assert body["title"].strip() != ""
        assert body["body"].strip() != ""

    def test_get_all_posts_returns_full_collection(self):
        """
        Given: the JSONPlaceholder API
        When: I GET the posts collection
        Then: it returns 200 and exactly 100 posts
        """
        response = requests.get(f"{BASE}/posts", timeout=TIMEOUT)

        assert response.status_code == 200
        assert len(response.json()) == 100

    def test_create_post_returns_201_and_echoes_payload(self):
        """
        Given: a new post payload
        When: I POST it to /posts
        Then: the API returns 201, echoes the payload, and assigns a numeric id
        """
        payload = {"title": "SDET portfolio", "body": "created via API test", "userId": 1}
        response = requests.post(f"{BASE}/posts", json=payload, timeout=TIMEOUT)

        assert response.status_code == 201
        body = response.json()
        assert body["title"] == payload["title"]
        assert body["body"] == payload["body"]
        assert isinstance(body["id"], int)
