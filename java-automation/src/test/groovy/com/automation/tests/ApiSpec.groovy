package com.automation.tests

import com.microsoft.playwright.APIRequest
import com.microsoft.playwright.APIRequestContext
import com.microsoft.playwright.APIResponse
import com.microsoft.playwright.Playwright
import com.microsoft.playwright.options.RequestOptions
import groovy.json.JsonOutput
import groovy.json.JsonSlurper
import spock.lang.Shared
import spock.lang.Specification

/**
 * REST API contract tests against JSONPlaceholder, written as a true Spock
 * specification — real given/when/then blocks and a data-driven `where:` table.
 * Uses Playwright's APIRequestContext (no browser launched).
 */
class ApiSpec extends Specification {

    static final String BASE = "https://jsonplaceholder.typicode.com"

    @Shared
    Playwright playwright

    @Shared
    APIRequestContext request

    def setupSpec() {
        playwright = Playwright.create()
        request = playwright.request().newContext(
            new APIRequest.NewContextOptions().setBaseURL(BASE))
    }

    def cleanupSpec() {
        request?.dispose()
        playwright?.close()
    }

    def "GET /posts/#postId returns a post matching the schema"() {
        when: "I request a single post by id"
        APIResponse response = request.get("/posts/${postId}")

        then: "the response is 200 and the body matches the expected schema"
        response.status() == 200
        def body = new JsonSlurper().parseText(response.text())
        body.id == postId
        body.userId instanceof Integer
        !body.title.trim().isEmpty()
        !body.body.trim().isEmpty()

        where: "the same contract holds for several posts"
        postId << [1, 2, 3]
    }

    def "GET /posts returns the full collection of 100 posts"() {
        when: "I request the posts collection"
        APIResponse response = request.get("/posts")

        then: "the response is 200 and contains exactly 100 posts"
        response.status() == 200
        new JsonSlurper().parseText(response.text()).size() == 100
    }

    def "POST /posts creates a resource and echoes the payload"() {
        given: "a new post payload"
        def payload = [title: "SDET portfolio", body: "created via API test", userId: 1]

        when: "I POST it to /posts"
        APIResponse response = request.post("/posts",
            RequestOptions.create()
                .setHeader("Content-Type", "application/json")
                .setData(JsonOutput.toJson(payload)))

        then: "the API returns 201, echoes the payload, and assigns a numeric id"
        response.status() == 201
        def body = new JsonSlurper().parseText(response.text())
        body.title == payload.title
        body.body == payload.body
        body.id instanceof Integer
    }
}
