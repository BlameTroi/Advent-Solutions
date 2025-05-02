/* solution.c -- aoc 2016 xx -- troy brumley */

#include <assert.h>
#include <limits.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "solution.h"

#define TXBMISC_IMPLEMENTATION
#include "txbmisc.h"
#define TXBRS_IMPLEMENTATION
#include "txbrs.h"
#define TXBSTR_IMPLEMENTATION
#include "txbstr.h"
#define TXBQU_IMPLEMENTATION
#include "txbqu.h"

node nodes_grid[ROW_DIM][COL_DIM];
int num_nodes = 0;
node *nodes_list = NULL;

void
load_grid(rscb *input) {

	/* clear any priors grid */
	memset(nodes_grid, 0, sizeof(nodes_grid));
	if (nodes_list != NULL) {
		free(nodes_list);
		nodes_list = NULL;
	}
	num_nodes = 0;

	/* create a string of all non-numeric characters for
	 * split_string(). input is positional and none of
	 * the verbiage matters for this problem.
	 *
	 * lines we care about start with /dev, and we only
	 * need the six numeric fields:
	 *
	 * Filesystem              Size  Used  Avail  Use%
	 * /dev/grid/node-x0-y0     90T   69T    21T   76%
	 * /dev/grid/node-x0-y1     88T   71T    17T   80%
	 *                 1  2      3     4      5     6   */

	char non_numerics[129];
	int i = 0;
	for (int c = ' '; c <= 127; c++) {
		if (!is_digit(c)) {
			non_numerics[i] = c;
			non_numerics[i+1] = '\0';
			i += 1;
		}
	}

	/* read, parse, store */

	char buffer[256];
	int buflen = 256;
	rs_rewind(input);
	while (!rs_at_end(input)) {
		if (rs_gets(input, buffer, buflen) && buffer[0] == '/') {
			const char **tokens = split_string(buffer, non_numerics);
			assert(tokens[1] && tokens[2] && tokens[3] && tokens[4] && tokens[5] &&
				tokens[6]);
			int col = strtol(tokens[1], NULL, 10);
			int row = strtol(tokens[2], NULL, 10);
			nodes_grid[row][col].loc = (loc) {
				col, row
			};
			nodes_grid[row][col].size  = strtol(tokens[3], NULL, 10);
			nodes_grid[row][col].used  = strtol(tokens[4], NULL, 10);
			nodes_grid[row][col].avail = strtol(tokens[5], NULL, 10);
			nodes_grid[row][col].pct   = strtol(tokens[6], NULL, 10);
			/* printf("%s\n", buffer); */
			/* node *n = &nodes_grid[row][col]; */
			/* printf("row%d-col%d size=%d used=%d avail=%d pct=%d\n", n->loc.col, n->loc.row, n->size, */
			/* 	n->used, n->avail, n->pct); */
			num_nodes += 1;
			free_split(tokens);
		}
	}

	/* get in a list to try something out */
	nodes_list = malloc(num_nodes *sizeof(node));
	i = 0;
	for (int row = 0; row < ROW_DIM; row ++)
		for (int col = 0; col < COL_DIM; col++) {
			nodes_list[i] = nodes_grid[row][col];
			i += 1;
		}
}

/*
 * to do this, you'd like to count the number of viable pairs of
 * nodes. a viable pair is any two nodes (a,b), regardless of whether
 * they are directly connected, such that:
 *
 * node a is not empty (its used is not zero). nodes a and b are not
 * the same node. the data on node a (its used) would fit on node b
 * (its avail).
 *
 * how many viable pairs of nodes are there?
 */

bool
viable_pairing(const node *a, const node *b) {
	if (a == b)
		return false;
	if (a->used == 0)
		return false;
	if (a->used > b->avail)
		return false;
	return true;
}

int
check_viable_with(const node *a) {
	if (a->used == 0)
		return 0;
	int viable = 0;
	for (int row = 0; row < ROW_DIM; row++)
		for (int col = 0; col < COL_DIM; col++)
			if (viable_pairing(a, &nodes_grid[row][col]))
				viable += 1;
	return viable;
}

int
viable_node_pairs(void) {
	int viable = 0;
	for (int row = 0; row < ROW_DIM; row++)
		for (int col = 0; col < COL_DIM; col++)
			viable += check_viable_with(&nodes_grid[row][col]);
	return viable;
}

int
viable_node_pairs2(void) {
	int viable = 0;

	for (int i = 0; i < num_nodes; i++)
		for (int j = i+1; j < num_nodes; j++) {
			if (nodes_list[i].used != 0 && nodes_list[i].used <= nodes_list[j].avail)
				viable += 1;
			if (nodes_list[j].used != 0 && nodes_list[j].used <= nodes_list[i].avail)
				viable += 1;
		}

	return viable;
}


/*
 * part one:
 */

int
part_one(
	const char *fname
) {

	FILE *ifile = fopen(fname, "r");
	if (!ifile) {
		fprintf(stderr, "error: could not open file: %s\n", fname);
		return EXIT_FAILURE;
	}

	rscb *rs = rs_create_string_from_file(ifile);
	load_grid(rs);

	printf("part one: %d\n", viable_node_pairs());
	printf("part one: %d\n", viable_node_pairs2());

	free(rs);
	fclose(ifile);
	return EXIT_SUCCESS;
}



/*
 * part two:
 */

int
part_two(
	const char *fname
) {
	FILE *ifile;

	ifile = fopen(fname, "r");
	if (!ifile) {
		fprintf(stderr, "error: could not open file: %s\n", fname);
		return EXIT_FAILURE;
	}

	rscb *rs = rs_create_string_from_file(ifile);
	load_grid(rs);

	/* printf("part one: %d\n", viable_node_pairs()); */
	/* printf("part one: %d\n", viable_node_pairs2()); */

	for (int row = 0; row < ROW_DIM; row++) {
		printf("\n%2d ", row);
		for (int col = 0; col < COL_DIM; col++) {
			if (nodes_grid[row][col].pct == 0)
				printf("0");
			else if (col == COL_DIM-1 && row == 0)
				printf("T");
			else if (col == 0 && row == 0)
				printf("G");
			else {
				char g = nodes_grid[row][col].pct / 10;
				if (g > 5 && g < 8) g = '.';
				if (g < 10) g = '0' + g;
				printf("%c", g);
			}
		}
	}
	printf("\n");

	free(rs);

	fclose(ifile);

	/* move goal to start, requires using an empty node
	   for handoff. data and goal are predefined, empty
	   must be found at runtime. in my data it was row 20 col 3.

	        loc loc;
		int size;
		int used;
		int avail;
		int pct;

	   move empty adjacent to target
	   move data from target to empty
	   move to start */

	loc start = (loc) {0, 0};
	loc target = (loc) {0, COL_DIM-1};
	/* adjacent to target */
	loc adj1 = target; adj1.row += 1;
	loc adj2 = target; adj2.col -= 1;
	loc empty = (loc) {-1, -1};

	for (int row = 0; row < ROW_DIM; row++)
		for (int col = 0; col < COL_DIM; col++)
			if (nodes_grid[row][col].pct == 0) {
				empty = (loc) {row, col};
				break;
			}
	assert(empty.row >= 0 && empty.col >= 0);

	printf("\nseeking to move data to (%2d, %2d)\n", start.row, start.col);
	printf("                   from (%2d, %2d)\n", target.row, target.col);
	printf("available empty node at (%2d, %2d)\n\n", empty.row, empty.col);

	/* there is only one path through row 7, at col 0, so get there first */
	int leg1 = INT_MAX;
	qucb *pending = qu_create();
	qu_enqueue(pending, &empty);
	while (!qu_empty(pending)) {

	}




	printf("part two: %d\n", 0);

	fclose(ifile);
	return EXIT_SUCCESS;
}
