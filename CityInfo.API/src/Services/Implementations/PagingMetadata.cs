namespace CityInfo.API.src.Services.Implementations{
    public class PagingMetadata{
        public int TotalItemCount {get; set;}
        public int TotalPageCount {get; set;}
        public int PageSize {get; set;}
        public int CurrentPage {get; set;}

        public PagingMetadata(int TotalItemCount,int PageSize,int CurrentPage
        ){
            this.TotalItemCount = TotalItemCount;
            this.PageSize = PageSize;
            this.CurrentPage = CurrentPage;

            this.TotalPageCount = (int) Math.Ceiling(this.TotalItemCount / (double) this.PageSize);

        }
    }
}