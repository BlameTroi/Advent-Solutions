/*  unittest.c -- shell for advent of code unit tests -- troy brumley */

/*  because you should always make an effort to test first! */

#include <assert.h>
#include <stdbool.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>

#include "minunit.h"
#include "solution.h"
#include "txbmisc.h"

void
test_setup(void) {
}

void
test_teardown(void) {
}

/*
 * sample test shell.
 */

MU_TEST(test_test) {
	point points[9];
	int n = find_points_in_maze(test_maze, points, 9);
	int m = sum_one_to(n - 1);
	int shortest_paths[m][3];
	int o = find_shortest_paths_between(test_maze, points, n, shortest_paths);
	mu_should(points[0].n == 0);
	mu_should(points[0].row == 1);
	mu_should(points[0].col == 1);
	mu_should(points[4].n == 4);
	mu_should(points[4].row == 3);
	mu_should(points[4].col == 1);
	mu_should(n == 5);
	printf("\n");
	for (int i = 0; i < o; i++)
		printf("%2d  from: %d  to: %d  distance: %3d\n", i, shortest_paths[i][0],
			shortest_paths[i][1], shortest_paths[i][2]);
	printf("\n");
}

/*
 * here we define the whole test suite.
 */
MU_TEST_SUITE(test_suite) {

	MU_SUITE_CONFIGURE(&test_setup, &test_teardown);

	/* run your tests here */
	MU_RUN_TEST(test_test);
}

/*
 * master control:
 */

int
main(int argc, char *argv[]) {
	MU_RUN_SUITE(test_suite);
	MU_REPORT();
	return MU_EXIT_CODE;
}
