using System.Collections.Generic;

namespace Wrapper {
	//I hope this doesnt break or else ill brek someone
	public class Deque<T> : List<T> {

		public Deque() { }
		public Deque(int capacity) : base(capacity) { }
		public Deque(IEnumerable<T> collection) : base(collection) { }

		public void push_end(T item) {
			Insert(Count, item);
		}
		public void push_start(T item) {
			Insert(0, item);
		}

		public void pop_end() {
			RemoveAt(Count - 1);
		}
		public void pop_start() {
			RemoveAt(0);
		}
	}
}
